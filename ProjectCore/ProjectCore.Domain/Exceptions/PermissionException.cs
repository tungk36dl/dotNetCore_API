namespace ProjectCore.Domain.Exceptions;

public abstract class PermissionException : DomainException
{
    protected PermissionException(string message) : base(message) { }
}

public sealed class PermissionNotFoundException : PermissionException
{
    public PermissionNotFoundException(IEnumerable<Guid> missingIds)
        : base($"Permission(s) not found: {string.Join(", ", missingIds)}") { }
}
