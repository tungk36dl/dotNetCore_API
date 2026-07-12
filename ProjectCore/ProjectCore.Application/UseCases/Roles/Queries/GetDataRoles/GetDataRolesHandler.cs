using MediatR;
using ProjectCore.Application.Common.Models;
using ProjectCore.Application.Dtos.Roles;
using ProjectCore.Application.Mappings;
using ProjectCore.Domain.Interfaces.RoleRepository;

namespace ProjectCore.Application.UseCases.Roles.Queries.GetDataRoles;

public sealed class GetDataRolesHandler : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetDataRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery query, CancellationToken cancellationToken)
    {
        var search = new RoleSearch
        {
            Keyword        = query.Keyword,
            Name           = query.Name,
            SortBy         = query.SortBy,
            SortDescending = query.SortDescending,
            Page           = query.Page,
            PageSize       = query.PageSize,
        };

        var (roles, count) = await _roleRepository.GetDataAsync(search, cancellationToken);
        var dtos = roles.Select(RoleMapper.ToDto).ToList();
        return new PagedResult<RoleDto>(dtos, count, query.Page, query.PageSize);
    }
}
