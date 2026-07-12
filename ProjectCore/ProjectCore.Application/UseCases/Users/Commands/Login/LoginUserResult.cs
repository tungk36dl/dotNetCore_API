using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.UseCases.Users.Commands.Login
{
    public sealed class LoginUserResult
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public IReadOnlyList<string> Permissions { get; init; } = [];
    }

}
