using MediatR;

namespace ProjectCore.Application.UseCases.Roles.Commands.CreateRole;

public sealed class CreateRoleCommand : IRequest<Guid>
{
    public string RoleName { get; init; } = default!;
    public string? Description { get; init; }
    public Guid CreatedBy { get; init; }
    public IReadOnlyList<Guid> PermissionIds { get; init; } = [];
}
