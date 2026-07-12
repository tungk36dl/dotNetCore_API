using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.RoleRepository;

namespace ProjectCore.Application.UseCases.Roles.Commands.DeleteRole;

public sealed class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Unit>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRoleHandler> _logger;

    public DeleteRoleHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteRoleHandler> logger)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new RoleNotFoundException(command.Id);

        var roleName = role.Name.Value;
        _roleRepository.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.RoleDeleted,
            "Role deleted. RoleId={RoleId} RoleName={RoleName}",
            command.Id, roleName);

        return Unit.Value;
    }
}
