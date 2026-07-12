using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Common.Security;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.UserRepository;

namespace ProjectCore.Application.UseCases.Users.Commands.Login;

public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPermissionQueryRepository _permissionQueryRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<LoginUserHandler> _logger;

    public LoginUserHandler(
        IUserRepository userRepository,
        IPermissionQueryRepository permissionQueryRepository,
        IPasswordHasher passwordHasher,
        ILogger<LoginUserHandler> logger)
    {
        _userRepository = userRepository;
        _permissionQueryRepository = permissionQueryRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUserNameOrEmailAsync(command.UserNameOrEmail, cancellationToken);

        if (user is null || !_passwordHasher.Verify(user.PasswordHash, command.Password))
        {
            _logger.LogWarning(LogEvents.AuthLoginFailed,
                "Login failed. Identifier={Identifier}",
                command.UserNameOrEmail);
            throw new InvalidLoginException();
        }

        var permissions = await _permissionQueryRepository
            .GetPermissionsByUserIdAsync(user.Id, cancellationToken);

        var permissionList = permissions.Select(p => p.ToString()).ToList();

        _logger.LogInformation(LogEvents.AuthLoginSuccess,
            "Login successful. UserId={UserId} UserName={UserName} PermissionCount={PermissionCount}",
            user.Id, user.UserName, permissionList.Count);

        return new LoginUserResult
        {
            UserId      = user.Id,
            UserName    = user.UserName.ToString(),
            Email       = user.Email.ToString(),
            Permissions = permissionList
        };
    }
}
