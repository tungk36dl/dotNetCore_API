using Microsoft.Extensions.Logging;

namespace ProjectCore.Application.Logging;

/// <summary>
/// Centralised EventId registry for structured logging.
/// Use with _logger.LogInformation(LogEvents.UserCreated, "User {UserName} created", userName)
///
/// ID ranges:
///   1000–1099  Auth
///   2000–2099  Role commands
///   2100–2199  Role queries
///   3000–3099  User commands
///   3100–3199  User queries
///   4000–4099  Permissions
///   5000–5099  System bootstrap / seed
///   6000–6099  MediatR pipeline
///   9000–9099  Errors / exceptions
/// </summary>
public static class LogEvents
{
    // ── Auth ──────────────────────────────────────────────────── 1000-1099 ──
    public static readonly EventId AuthLoginSuccess   = new(1001, "AUTH_LOGIN_SUCCESS");
    public static readonly EventId AuthLoginFailed    = new(1002, "AUTH_LOGIN_FAILED");
    public static readonly EventId AuthTokenRefreshed = new(1003, "AUTH_TOKEN_REFRESHED");
    public static readonly EventId AuthLogout         = new(1004, "AUTH_LOGOUT");

    // ── Role Commands ─────────────────────────────────────────── 2000-2099 ──
    public static readonly EventId RoleCreated        = new(2001, "ROLE_CREATED");
    public static readonly EventId RoleUpdated        = new(2002, "ROLE_UPDATED");
    public static readonly EventId RoleDeleted        = new(2003, "ROLE_DELETED");

    // ── Role Queries ──────────────────────────────────────────── 2100-2199 ──
    public static readonly EventId RoleQueried        = new(2101, "ROLE_QUERIED");

    // ── User Commands ─────────────────────────────────────────── 3000-3099 ──
    public static readonly EventId UserCreated        = new(3001, "USER_CREATED");
    public static readonly EventId UserUpdated        = new(3002, "USER_UPDATED");
    public static readonly EventId UserDeleted        = new(3003, "USER_DELETED");

    // ── User Queries ──────────────────────────────────────────── 3100-3199 ──
    public static readonly EventId UserQueried        = new(3101, "USER_QUERIED");

    // ── Permissions ───────────────────────────────────────────── 4000-4099 ──
    public static readonly EventId PermissionsSynced  = new(4001, "PERMISSIONS_SYNCED");
    public static readonly EventId PermissionsAdded   = new(4002, "PERMISSIONS_ADDED");

    // ── System Bootstrap ──────────────────────────────────────── 5000-5099 ──
    public static readonly EventId BootstrapStarted   = new(5001, "BOOTSTRAP_STARTED");
    public static readonly EventId BootstrapCompleted = new(5002, "BOOTSTRAP_COMPLETED");
    public static readonly EventId BootstrapFailed    = new(5003, "BOOTSTRAP_FAILED");

    // ── MediatR Pipeline ──────────────────────────────────────── 6000-6099 ──
    public static readonly EventId RequestStarted     = new(6001, "REQUEST_STARTED");
    public static readonly EventId RequestCompleted   = new(6002, "REQUEST_COMPLETED");
    public static readonly EventId RequestFailed      = new(6003, "REQUEST_FAILED");

    // ── Errors / Exceptions ───────────────────────────────────── 9000-9099 ──
    public static readonly EventId UnhandledException = new(9001, "UNHANDLED_EXCEPTION");
    public static readonly EventId DomainViolation    = new(9002, "DOMAIN_VIOLATION");
    public static readonly EventId AuthViolation      = new(9003, "AUTH_VIOLATION");
}
