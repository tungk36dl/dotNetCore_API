using MediatR;
using ProjectCore.Application.Dtos.Roles;
using ProjectCore.Application.Mappings;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.RoleRepository;

namespace ProjectCore.Application.UseCases.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleByIdHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDto> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new RoleNotFoundException(query.Id);

        return RoleMapper.ToDto(role);
    }
}
