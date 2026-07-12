using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.Exceptions
{
    public abstract class RoleException : DomainException
    {
        protected RoleException(string message)
            : base(message)
        {
        }
    }

    public sealed class RoleNotFoundException : RoleException
    {
        public RoleNotFoundException(Guid roleId)
            : base($"Role with ID '{roleId}' was not found.")
        {
        }
    }   

    public sealed class RoleNameAlreadyExistsException : RoleException
    {
        public RoleNameAlreadyExistsException(string roleName)
            : base($"Role with name '{roleName}' already exists.")
        {
        }
    }


    public sealed class InvalidRoleOperationException : RoleException
    {
        public InvalidRoleOperationException(string message)
            : base(message)
        {
        }
    }

    public sealed class RoleInUseException : RoleException
    {
        public RoleInUseException(string roleName)
            : base($"Role '{roleName}' is currently assigned to users and cannot be deleted.")
        {
        }
    }
    public sealed class CannotDeleteAdminRoleException : RoleException
    {
        public CannotDeleteAdminRoleException()
            : base("The 'Admin' role cannot be deleted.")
        {
        }
    }

}
