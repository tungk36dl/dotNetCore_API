using MediatR;
using ProjectCore.Application.Dtos.Roles;

namespace ProjectCore.Application.UseCases.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>;
