using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.Dtos.Users
{
    public sealed class AuthUserDto
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public IReadOnlyList<string> Roles { get; init; } = [];
        public IReadOnlyList<string> Permissions { get; init; } = [];
    }
}
