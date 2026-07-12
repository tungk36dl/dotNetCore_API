using System.ComponentModel.DataAnnotations;

namespace ProjectCore.Presentation.API.Models.Requests;

public sealed class RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
