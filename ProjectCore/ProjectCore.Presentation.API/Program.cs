using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectCore.Application;
using ProjectCore.Application.UseCases.Permissions.Scan;
using ProjectCore.Application.UseCases.SeedData;
using ProjectCore.Infrastructure;
using ProjectCore.Presentation.API.Authentication;
using ProjectCore.Presentation.API.Middleware;
using ProjectCore.Presentation.API.Permissions;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

// Load .env file if exists
Env.Load();

// ========== Serilog Bootstrap ==========
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up application...");

var builder = WebApplication.CreateBuilder(args);

// ========== Serilog (full config: Console + File + Loki) ==========
builder.Host.UseSerilog((context, services, cfg) =>
{
    cfg .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "ProjectCore.API")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

    // Push logs to Grafana Loki when configured (docker / production)
    var lokiUri = context.Configuration["Loki:Uri"];
    if (!string.IsNullOrEmpty(lokiUri))
    {
        cfg.WriteTo.GrafanaLoki(
            lokiUri,
            labels:              [new LokiLabel { Key = "app",         Value = "projectcore-api" },
                                  new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName }],
            propertiesAsLabels:  ["Level", "SourceContext"]);

        Log.Information("Loki sink registered. Uri={LokiUri}", lokiUri);
    }
});

// ========== Controllers ==========
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(err => err.ErrorMessage))
                .ToList();

            var response = ProjectCore.Presentation.API.Models.Responses
                .ApiResponse.Fail("Validation failed", errors);

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
        };
    });

// ========== Swagger/OpenAPI ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "ProjectCore API",
        Version     = "v1",
        Description = "API for ProjectCore - User & Role Management"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ========== JWT Authentication ==========
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var secretKey = jwtSettings["SecretKey"]!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtSettings["Issuer"],
        ValidAudience            = jwtSettings["Audience"],
        IssuerSigningKey         = key,
        ClockSkew                = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ========== CORS ==========
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (allowedOrigins is null or { Length: 0 })
{
    if (!builder.Environment.IsDevelopment())
        Log.Warning("Cors:AllowedOrigins is not configured — falling back to localhost:3000 only. Production frontend calls will be blocked.");
    allowedOrigins = ["http://localhost:3000", "https://localhost:3000"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJs", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ========== OpenTelemetry — Tracing → Grafana Tempo ==========
var otlpEndpoint = builder.Configuration["Otel:Endpoint"];

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(
        serviceName:    "ProjectCore.API",
        serviceVersion: "1.0.0"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(o => o.RecordException = true)
            .AddHttpClientInstrumentation();

        if (!string.IsNullOrEmpty(otlpEndpoint))
        {
            tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint));
            Log.Information("OTLP trace exporter registered. Endpoint={OtlpEndpoint}", otlpEndpoint);
        }
    });

// ========== Application & Infrastructure DI ==========
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// ========== API-specific services ==========
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPermissionScanner, ApiPermissionScanner>();

var app = builder.Build();

// ========== Seed Data ==========
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Log.Information("Resolving SeedDataHandler from DI...");
        var seedDataHandler = services.GetRequiredService<SeedDataHandler>();
        await seedDataHandler.SeedAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Seed data FAILED on startup");
    }
}

// ========== Middleware Pipeline ==========
app.UseSerilogRequestLogging(o =>
{
    o.EnrichDiagnosticContext = (diag, http) =>
    {
        diag.Set("RequestHost",   http.Request.Host.Value);
        diag.Set("RequestScheme", http.Request.Scheme);
        diag.Set("UserAgent",     http.Request.Headers.UserAgent.ToString());
    };
});

// Swagger JSON spec available in all environments (internal tooling / CI).
// UI only in Development to avoid leaking API surface.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectCore API v1");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
    });
}

app.UseMiddleware<ExceptionMiddleware>();

// CORS before HttpsRedirection — preflight OPTIONS must receive CORS headers,
// not a 307 redirect, or the browser will reject the response.
app.UseCors("NextJs");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ========== Prometheus metrics endpoint ==========
// Exposes /metrics for Prometheus scraping.
// Place after auth so /metrics is accessible without JWT (standard practice).
app.UseHttpMetrics();
app.MapMetrics();          // GET /metrics

app.MapControllers();

app.Run();
