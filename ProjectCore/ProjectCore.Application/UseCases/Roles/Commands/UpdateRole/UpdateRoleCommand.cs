using MediatR;

namespace ProjectCore.Application.UseCases.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public Guid UpdatedBy { get; init; }
    /// <summary>null = không thay đổi; [] = xóa hết; [ids] = sync.</summary>
    public IReadOnlyList<Guid>? PermissionIds { get; init; }
}
