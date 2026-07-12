namespace ProjectCore.Presentation.API.Models.Requests;

public sealed class UpdateRoleRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    /// <summary>
    /// Danh sách permission ID mới cho role.
    /// - null = không thay đổi permissions hiện tại.
    /// - [] (empty) = xóa toàn bộ permissions.
    /// - [id1, id2] = sync: thêm thiếu, xóa thừa.
    /// </summary>
    public List<Guid>? PermissionIds { get; set; }
}
