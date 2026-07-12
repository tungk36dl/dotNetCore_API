using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Domain.Entities;

public class UserRole : DomainEntity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public DateTime AssignedAt { get; private set; }
    public Guid AssignedBy { get; private set; }

    protected UserRole() { }

    internal UserRole(Guid id, Guid userId, Guid roleId, Guid assignedBy, Guid createdBy) : base(id, createdBy)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy;
        AssignedAt = DateTime.UtcNow;
    }
    internal UserRole( Guid userId, Guid roleId, Guid assignedBy)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        RoleId = roleId;
        AssignedBy = assignedBy;
        CreatedBy = assignedBy;
        AssignedAt = DateTime.UtcNow;
        CreatedDate = DateTime.UtcNow;
    }
}

