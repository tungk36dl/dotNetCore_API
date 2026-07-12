namespace ProjectCore.Presentation.API.Models.Requests;

public sealed class LogoutRequest
{
    public string? RefreshToken { get; set; }
}
