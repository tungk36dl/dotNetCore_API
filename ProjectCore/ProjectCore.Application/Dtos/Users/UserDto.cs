using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.Dtos.Users
{
    public sealed class UserDto : DomainDto
    {
        public Guid Id { get; init; }

        public string UserName { get; init; }
        public string Email { get; init; }

        public string? FullName { get; init; }
        public string? PhoneNumber { get; init; }

        public string? Gender { get; init; }   // ⭐
        public DateOnly? DateOfBirth { get; init; }

        public string? Address { get; init; }
        public string? AvatarUrl { get; init; }

        public IReadOnlyCollection<Guid> RoleIds { get; init; }
    }

}
