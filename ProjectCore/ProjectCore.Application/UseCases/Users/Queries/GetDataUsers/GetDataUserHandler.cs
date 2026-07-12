using MediatR;
using ProjectCore.Application.Common.Models;
using ProjectCore.Application.Dtos.Users;
using ProjectCore.Application.Mappings;
using ProjectCore.Domain.Interfaces.UserRepository;

namespace ProjectCore.Application.UseCases.Users.Queries.GetDataUsers;

public sealed class GetDataUserHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetDataUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var search = new UserSearch
        {
            Keyword        = query.Keyword,
            UserName       = query.UserName,
            Email          = query.Email,
            FullName       = query.FullName,
            Gender         = query.Gender,
            RoleId         = query.RoleId,
            SortBy         = query.SortBy,
            SortDescending = query.SortDescending,
            Page           = query.Page,
            PageSize       = query.PageSize,
        };

        var (users, count) = await _userRepository.GetDataAsync(search, cancellationToken);
        var dtos = users.Select(UserMapper.ToDto).ToList();
        return new PagedResult<UserDto>(dtos, count, query.Page, query.PageSize);
    }
}
