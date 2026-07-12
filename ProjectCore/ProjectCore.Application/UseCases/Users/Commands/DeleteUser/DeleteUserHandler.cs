using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.UserRepository;

namespace ProjectCore.Application.UseCases.Users.Commands.DeleteUser;

public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteUserHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new UserNotFoundException();

        var userName = user.UserName.ToString();
        _userRepository.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.UserDeleted,
            "User deleted. UserId={UserId} UserName={UserName}",
            command.Id, userName);

        return Unit.Value;
    }
}
