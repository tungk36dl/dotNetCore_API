using MediatR;
using ProjectCore.Application.Dtos.Users;

namespace ProjectCore.Application.UseCases.Users.Queries.GetUserById;

public sealed class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; init; }
}
