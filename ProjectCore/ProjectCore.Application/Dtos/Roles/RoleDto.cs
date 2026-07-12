using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.Dtos.Roles
{
    public sealed class RoleDto : DomainDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public IReadOnlyCollection<Guid> PermissionIds { get; set; }
    }
}
