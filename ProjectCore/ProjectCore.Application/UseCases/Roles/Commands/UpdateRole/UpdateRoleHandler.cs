using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.Interfaces.PermissionRepository;
using ProjectCore.Domain.Interfaces.RoleRepository;

namespace ProjectCore.Application.UseCases.Roles.Commands.UpdateRole;

public sealed class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, Unit>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRoleHandler> _logger;

    public UpdateRoleHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateRoleHandler> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new RoleNotFoundException(command.Id);

        // Entity is already tracked — no _roleRepository.Update() to avoid EF cascading Modified state.
        role.UpdateDetails(command.Name, command.Description, command.UpdatedBy);

        var addedCount = 0;
        var removedCount = 0;

        if (command.PermissionIds is not null)
        {
            var newIds = command.PermissionIds.ToHashSet();

            if (newIds.Count > 0)
            {
                var existingIds = await _permissionRepository.GetExistingIdsAsync(newIds, cancellationToken);
                var missingIds = newIds.Except(existingIds).ToList();
                if (missingIds.Count > 0)
                    throw new PermissionNotFoundException(missingIds);
            }

            var currentIds = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
            var toRemove = currentIds.Except(newIds).ToList();
            var toAdd    = newIds.Except(currentIds).ToList();

            foreach (var id in toRemove) role.RemovePermission(id);
            foreach (var id in toAdd)    role.AddPermission(id, command.UpdatedBy);

            addedCount   = toAdd.Count;
            removedCount = toRemove.Count;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.RoleUpdated,
            "Role updated. RoleId={RoleId} PermissionsAdded={Added} PermissionsRemoved={Removed} UpdatedBy={UpdatedBy}",
            role.Id, addedCount, removedCount, command.UpdatedBy);

        return Unit.Value;
    }
}
