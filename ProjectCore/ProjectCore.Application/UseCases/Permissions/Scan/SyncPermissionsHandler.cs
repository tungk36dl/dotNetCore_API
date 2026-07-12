using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.Logging;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Interfaces.PermissionRepository;

namespace ProjectCore.Application.UseCases.Permissions.Scan;

public sealed class SyncPermissionsHandler : IRequestHandler<SyncPermissionsCommand, Unit>
{
    private readonly IPermissionScanner _scanner;
    private readonly IPermissionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SyncPermissionsHandler> _logger;

    public SyncPermissionsHandler(
        IPermissionScanner scanner,
        IPermissionRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<SyncPermissionsHandler> logger)
    {
        _scanner = scanner;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(SyncPermissionsCommand command, CancellationToken cancellationToken)
    {
        var permissions = _scanner.Scan().ToList();
        var newCount = 0;

        foreach (var p in permissions)
        {
            if (await _repository.ExistsAsync(p.Module, p.Action, cancellationToken))
                continue;

            var permission = new Permission(Guid.NewGuid(), p.Module, p.Action, command.IssuedByUserId);
            await _repository.AddAsync(permission, cancellationToken);
            newCount++;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(LogEvents.PermissionsSynced,
            "Permissions synced. ScannedCount={ScannedCount} NewCount={NewCount} IssuedBy={IssuedBy}",
            permissions.Count, newCount, command.IssuedByUserId);

        return Unit.Value;
    }
}
