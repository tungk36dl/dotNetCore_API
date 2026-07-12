using System.ComponentModel.DataAnnotations;

namespace ProjectCore.Presentation.API.Models.Requests;

public sealed class CreateRoleRequest
{
    [Required]
    public string RoleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Danh sách permission ID gán cho role khi tạo mới. Bỏ trống = không gán permission nào.
    /// </summary>
    public List<Guid>? PermissionIds { get; set; }
}
