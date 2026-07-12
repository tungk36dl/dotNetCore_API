using MediatR;

namespace ProjectCore.Application.UseCases.Roles.Commands.DeleteRole;

public sealed class DeleteRoleCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
