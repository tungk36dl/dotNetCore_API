namespace ProjectCore.Domain.Exceptions;

public abstract class UserDomainException : DomainException
{
    protected UserDomainException(string message)
        : base(message)
    {
    }
}

public sealed class UserAlreadyHasRoleException : UserDomainException
{
    public UserAlreadyHasRoleException()
        : base("User already has this role.") { }
}

public sealed class UserDoesNotHaveRoleException : UserDomainException
{
    public UserDoesNotHaveRoleException()
        : base("User does not have this role.") { }
}

public sealed class UserMustHaveAtLeastOneRoleException : UserDomainException
{
    public UserMustHaveAtLeastOneRoleException()
        : base("User must have at least one role.") { }
}

public sealed class UserEmailAlreadyExistsException : UserDomainException
{
    public UserEmailAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.") { }
}

public sealed class UserNameAlreadyExistsException : UserDomainException
{
    public UserNameAlreadyExistsException(string userName)
        : base($"User with username '{userName}' already exists.") { }
}

public sealed class UserNotFoundException : UserDomainException
{
    public UserNotFoundException() : base("User not found!") { }
}

public sealed class InvalidLoginException : DomainException
{
    public InvalidLoginException()
        : base("Invalid username/email or password.") { }
}




