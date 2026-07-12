using MediatR;
using ProjectCore.Application.Common.Models;
using ProjectCore.Application.Dtos.Users;

namespace ProjectCore.Application.UseCases.Users.Queries.GetDataUsers;

/// <summary>Paginated + filtered user list query (replaces raw UserSearch input on handler).</summary>
public sealed class GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public string? Keyword { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    public string? Gender { get; init; }
    public Guid? RoleId { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
