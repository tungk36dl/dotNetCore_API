using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.PermissionRepository;
using ProjectCore.Domain.Interfaces.RoleRepository;
using ProjectCore.Domain.ValueObjects.Role;

namespace ProjectCore.Application.UseCases.Roles.Commands.CreateRole;

public sealed class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRoleHandler> _logger;

    public CreateRoleHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateRoleHandler> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if (await _roleRepository.ExistsByNameAsync(new RoleName(command.RoleName), cancellationToken))
            throw new RoleNameAlreadyExistsException($"Role '{command.RoleName}' already exists.");

        if (command.PermissionIds.Count > 0)
        {
            var existingIds = await _permissionRepository.GetExistingIdsAsync(command.PermissionIds, cancellationToken);
            var missingIds = command.PermissionIds.Except(existingIds).ToList();
            if (missingIds.Count > 0)
                throw new PermissionNotFoundException(missingIds);
        }

        var role = new Role(
            Guid.NewGuid(),
            new RoleName(command.RoleName),
            command.Description,
            command.CreatedBy);

        foreach (var permissionId in command.PermissionIds)
            role.AddPermission(permissionId, command.CreatedBy);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.RoleCreated,
            "Role created. RoleId={RoleId} RoleName={RoleName} PermissionCount={PermissionCount} CreatedBy={CreatedBy}",
            role.Id, command.RoleName, command.PermissionIds.Count, command.CreatedBy);

        return role.Id;
    }
}
