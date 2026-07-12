using MediatR;

namespace ProjectCore.Application.UseCases.Users.Commands.CreateUser;

public sealed class CreateUserCommand : IRequest<Guid>
{
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    /// <summary>Plain-text password. Hashing is handled inside the handler.</summary>
    public string Password { get; init; } = default!;
    public Guid CreatedBy { get; init; }
}
