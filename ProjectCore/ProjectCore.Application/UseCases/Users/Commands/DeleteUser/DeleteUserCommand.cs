using MediatR;

namespace ProjectCore.Application.UseCases.Users.Commands.DeleteUser;

public sealed class DeleteUserCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}
