using System.Collections.Concurrent;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectCore.Application.Logging;
using ProjectCore.Application.UseCases.Users.Commands.Login;
using ProjectCore.Presentation.API.Authentication;
using ProjectCore.Presentation.API.Models.Requests;
using ProjectCore.Presentation.API.Models.Responses;

namespace ProjectCore.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    // In-memory refresh token store.
    // TODO: replace with a persistent store (DB / Redis) before production
    //       to survive restarts and support horizontal scaling.
    private static readonly ConcurrentDictionary<string, RefreshTokenEntry> _refreshTokens = new();

    public AuthController(IMediator mediator, IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>Login and receive JWT access + refresh tokens.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginUserCommand
        {
            UserNameOrEmail = request.UserNameOrEmail,
            Password        = request.Password,
        }, cancellationToken);

        var accessToken  = _jwtTokenService.GenerateAccessToken(
            result.UserId, result.UserName, result.Email, result.Permissions);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        _refreshTokens[refreshToken] = new RefreshTokenEntry
        {
            UserId      = result.UserId,
            UserName    = result.UserName,
            Email       = result.Email,
            Permissions = result.Permissions,
            ExpiresAt   = DateTime.UtcNow.AddDays(7),
        };

        _logger.LogInformation(LogEvents.AuthLoginSuccess,
            "Token issued. UserId={UserId} UserName={UserName}",
            result.UserId, result.UserName);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken,
            User = new AuthUserInfo
            {
                Id          = result.UserId,
                UserName    = result.UserName,
                Email       = result.Email,
                Permissions = result.Permissions,
            },
        }, "Login successful"));
    }

    /// <summary>Exchange an expired access token + valid refresh token for new tokens.</summary>
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var principal = _jwtTokenService.ValidateExpiredToken(request.AccessToken);
        if (principal is null)
        {
            _logger.LogWarning(LogEvents.AuthViolation,
                "Token refresh rejected — invalid access token.");
            return Unauthorized(ApiResponse.Fail("Invalid access token"));
        }

        if (!_refreshTokens.TryGetValue(request.RefreshToken, out var entry) ||
            entry.ExpiresAt < DateTime.UtcNow)
        {
            _refreshTokens.TryRemove(request.RefreshToken, out _);
            _logger.LogWarning(LogEvents.AuthViolation,
                "Token refresh rejected — invalid or expired refresh token. UserId={UserId}",
                principal.FindFirstValue(ClaimTypes.NameIdentifier));
            return Unauthorized(ApiResponse.Fail("Invalid or expired refresh token"));
        }

        var newAccessToken  = _jwtTokenService.GenerateAccessToken(
            entry.UserId, entry.UserName, entry.Email, entry.Permissions);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        _refreshTokens.TryRemove(request.RefreshToken, out _);
        _refreshTokens[newRefreshToken] = entry with { ExpiresAt = DateTime.UtcNow.AddDays(7) };

        _logger.LogInformation(LogEvents.AuthTokenRefreshed,
            "Token refreshed. UserId={UserId} UserName={UserName}",
            entry.UserId, entry.UserName);

        return Ok(ApiResponse<RefreshTokenResponse>.Ok(new RefreshTokenResponse
        {
            AccessToken  = newAccessToken,
            RefreshToken = newRefreshToken,
        }, "Token refreshed"));
    }

    /// <summary>Return current user info decoded from the access token claims.</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(ApiResponse<AuthUserInfo>.Ok(new AuthUserInfo
        {
            Id          = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            UserName    = User.FindFirstValue(ClaimTypes.Name)!,
            Email       = User.FindFirstValue(ClaimTypes.Email)!,
            Permissions = User.FindAll("permission").Select(c => c.Value).ToList(),
        }));
    }

    /// <summary>Logout — invalidate the provided refresh token.</summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout([FromBody] LogoutRequest? request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (request?.RefreshToken is not null)
            _refreshTokens.TryRemove(request.RefreshToken, out _);

        _logger.LogInformation(LogEvents.AuthLogout,
            "User logged out. UserId={UserId}", userId);

        return Ok(ApiResponse.Ok("Logged out successfully"));
    }

    private sealed record RefreshTokenEntry
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public IReadOnlyList<string> Permissions { get; init; } = [];
        public DateTime ExpiresAt { get; init; }
    }
}
