using MediatR;
using ProjectCore.Application.UseCases.Users.Commands.Login;

namespace ProjectCore.Application.UseCases.Users.Commands.Login;

public sealed class LoginUserCommand : IRequest<LoginUserResult>
{
    public string UserNameOrEmail { get; init; } = default!;
    public string Password { get; init; } = default!;
}
