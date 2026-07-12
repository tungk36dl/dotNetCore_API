# ── Stage 1: restore & build ──────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first (layer-cache friendly)
COPY ProjectCore.Domain/ProjectCore.Domain.csproj                               ProjectCore.Domain/
COPY ProjectCore.Application/ProjectCore.Application.csproj                     ProjectCore.Application/
COPY ProjectCore.Infrastructure/ProjectCore.Infrastructure.csproj               ProjectCore.Infrastructure/
COPY ProjectCore.Presentation.API/ProjectCore.Presentation.API.csproj           ProjectCore.Presentation.API/

RUN dotnet restore ProjectCore.Presentation.API/ProjectCore.Presentation.API.csproj

# Copy remaining source
COPY . .

RUN dotnet publish ProjectCore.Presentation.API/ProjectCore.Presentation.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ── Stage 2: runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Non-root user (security best practice)
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# ASP.NET Core listens on 8080 inside the container (default non-root port)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProjectCore.Presentation.API.dll"]
