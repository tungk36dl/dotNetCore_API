using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.UserRepository;
using ProjectCore.Domain.ValueObjects.User;

namespace ProjectCore.Application.UseCases.Users.Commands.UpdateUser;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new UserNotFoundException();

        Gender? gender = null;
        if (!string.IsNullOrEmpty(command.Gender) && Enum.TryParse<Gender>(command.Gender, out var g))
            gender = g;

        // Entity is already tracked by EF — calling _userRepository.Update() here cascades
        // Modified state to new child entities (UserRoles) causing DbUpdateConcurrencyException.
        // EF change tracker auto-detects mutation on SaveChanges.
        user.UpdateProfile(
            command.FullName    != null ? new FullName(command.FullName)       : null,
            command.PhoneNumber != null ? new PhoneNumber(command.PhoneNumber) : null,
            gender,
            command.DateOfBirth,
            command.Address,
            command.AvatarUrl   != null ? Avatar.FromUrl(command.AvatarUrl)    : null,
            command.UpdatedBy);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.UserUpdated,
            "User updated. UserId={UserId} UpdatedBy={UpdatedBy}",
            command.Id, command.UpdatedBy);

        return Unit.Value;
    }
}
