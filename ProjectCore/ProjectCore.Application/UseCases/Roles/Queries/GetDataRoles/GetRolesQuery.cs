using MediatR;
using ProjectCore.Application.Common.Models;
using ProjectCore.Application.Dtos.Roles;

namespace ProjectCore.Application.UseCases.Roles.Queries.GetDataRoles;

/// <summary>Paginated + filtered role list query (replaces raw RoleSearch input on handler).</summary>
public sealed class GetRolesQuery : IRequest<PagedResult<RoleDto>>
{
    public string? Keyword { get; init; }
    public string? Name { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
