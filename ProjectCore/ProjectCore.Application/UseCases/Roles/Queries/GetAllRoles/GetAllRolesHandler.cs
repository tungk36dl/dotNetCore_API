using MediatR;
using ProjectCore.Application.Dtos.Roles;
using ProjectCore.Application.Mappings;
using ProjectCore.Domain.Interfaces.RoleRepository;

namespace ProjectCore.Application.UseCases.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetAllRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(RoleMapper.ToDto);
    }
}
