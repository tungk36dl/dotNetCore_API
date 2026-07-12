using MediatR;

namespace ProjectCore.Application.UseCases.Permissions.Scan;

/// <summary>Scans API controllers and synchronises discovered permissions to the database.</summary>
public sealed class SyncPermissionsCommand : IRequest<Unit>
{
    public Guid IssuedByUserId { get; init; }
}
