using MediatR;
using ProjectCore.Application.Dtos.Roles;

namespace ProjectCore.Application.UseCases.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQuery : IRequest<RoleDto>
{
    public Guid Id { get; init; }
}
