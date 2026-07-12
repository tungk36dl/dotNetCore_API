using System.Net;
using System.Text.Json;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Presentation.API.Models.Responses;

namespace ProjectCore.Presentation.API.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            // Domain: Not Found (404)
            UserNotFoundException       => (HttpStatusCode.NotFound,              exception.Message),
            RoleNotFoundException       => (HttpStatusCode.NotFound,              exception.Message),

            // Domain: Conflict (409)
            UserEmailAlreadyExistsException  => (HttpStatusCode.Conflict,         exception.Message),
            UserNameAlreadyExistsException   => (HttpStatusCode.Conflict,         exception.Message),
            RoleNameAlreadyExistsException   => (HttpStatusCode.Conflict,         exception.Message),

            // Domain: Bad Request (400)
            RoleInUseException               => (HttpStatusCode.BadRequest,       exception.Message),
            CannotDeleteAdminRoleException   => (HttpStatusCode.BadRequest,       exception.Message),
            InvalidRoleOperationException    => (HttpStatusCode.BadRequest,       exception.Message),
            UserAlreadyHasRoleException      => (HttpStatusCode.BadRequest,       exception.Message),
            UserDoesNotHaveRoleException     => (HttpStatusCode.BadRequest,       exception.Message),
            UserMustHaveAtLeastOneRoleException => (HttpStatusCode.BadRequest,    exception.Message),

            // Domain: Unauthorized (401)
            InvalidLoginException            => (HttpStatusCode.Unauthorized,     exception.Message),

            // Domain: Unprocessable (422)
            PermissionNotFoundException      => (HttpStatusCode.UnprocessableEntity, exception.Message),

            // Catch-all DomainException → 400
            DomainException                  => (HttpStatusCode.BadRequest,       exception.Message),

            // Framework
            UnauthorizedAccessException      => (HttpStatusCode.Unauthorized,     "Unauthorized"),
            KeyNotFoundException             => (HttpStatusCode.NotFound,          "Resource not found"),
            ArgumentException ex             => (HttpStatusCode.BadRequest,        ex.Message),
            InvalidOperationException ex     => (HttpStatusCode.BadRequest,        ex.Message),

            _                                => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        // Domain violations → Warning (expected business errors, not bugs)
        // Unhandled exceptions → Error (unexpected, needs investigation)
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(LogEvents.UnhandledException, exception,
                "Unhandled exception. Path={Path} Method={Method} StatusCode={StatusCode}",
                context.Request.Path, context.Request.Method, (int)statusCode);
        }
        else if (statusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning(LogEvents.AuthViolation,
                "Auth violation. Path={Path} Method={Method} Error={Error}",
                context.Request.Path, context.Request.Method, exception.Message);
        }
        else
        {
            _logger.LogWarning(LogEvents.DomainViolation,
                "Domain violation. Path={Path} Method={Method} ExceptionType={ExceptionType} Error={Error}",
                context.Request.Path, context.Request.Method, exception.GetType().Name, exception.Message);
        }

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.Fail(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
