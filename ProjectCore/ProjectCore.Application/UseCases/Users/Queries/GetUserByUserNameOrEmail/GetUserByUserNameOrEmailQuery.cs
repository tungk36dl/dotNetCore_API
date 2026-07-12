using MediatR;
using ProjectCore.Application.Dtos.Users;

namespace ProjectCore.Application.UseCases.Users.Queries.GetUserByUserNameOrEmail;

public sealed class GetUserByUserNameOrEmailQuery : IRequest<UserDto>
{
    public string UserNameOrEmail { get; init; } = default!;
}
