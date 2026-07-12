using MediatR;

namespace ProjectCore.Application.UseCases.Users.Commands.UpdateUser;

public sealed class UpdateUserCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Gender { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? Address { get; init; }
    public string? AvatarUrl { get; init; }
    public Guid UpdatedBy { get; init; }
}
