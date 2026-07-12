namespace ProjectCore.Presentation.API.Models.Responses;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public AuthUserInfo User { get; init; } = default!;
}

public sealed class AuthUserInfo
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public IReadOnlyList<string> Permissions { get; init; } = [];
}

public sealed class RefreshTokenResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
}
