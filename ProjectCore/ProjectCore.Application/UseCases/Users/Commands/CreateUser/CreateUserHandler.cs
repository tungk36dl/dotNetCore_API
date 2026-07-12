using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Common.Security;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.UserRepository;
using ProjectCore.Domain.ValueObjects.User;

namespace ProjectCore.Application.UseCases.Users.Commands.CreateUser;

public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<CreateUserHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(new Email(command.Email), cancellationToken))
            throw new UserEmailAlreadyExistsException(command.Email);

        if (await _userRepository.ExistsByUserNameAsync(new UserName(command.UserName), cancellationToken))
            throw new UserNameAlreadyExistsException(command.UserName);

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = new User(
            Guid.NewGuid(),
            new UserName(command.UserName),
            new Email(command.Email),
            passwordHash,
            command.CreatedBy);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.UserCreated,
            "User created. UserId={UserId} UserName={UserName} Email={Email} CreatedBy={CreatedBy}",
            user.Id, command.UserName, command.Email, command.CreatedBy);

        return user.Id;
    }
}
