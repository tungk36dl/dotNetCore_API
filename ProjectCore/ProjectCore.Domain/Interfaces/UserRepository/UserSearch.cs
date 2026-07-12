using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.Interfaces.UserRepository
{
    public class UserSearch : SearchBase
    {
        public string? UserName { get; init; }
        public string? Email { get; init; }
        public string? FullName { get; init; }
        public string? Gender { get; init; }
        public Guid? RoleId { get; init; }
    }
}
