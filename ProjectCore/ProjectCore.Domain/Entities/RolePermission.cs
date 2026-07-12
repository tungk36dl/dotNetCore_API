using ProjectCore.Domain.Entities;

namespace ProjectCore.Domain.Entities
{
    public class RolePermission : DomainEntity<Guid>
    {
        public Guid RoleId { get; private set; }
        public Guid PermissionId { get; private set; }

        protected RolePermission() { }

        internal RolePermission(Guid roleId, Guid permissionId, Guid id, Guid createdBy) : base(id, createdBy)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }

        internal RolePermission(Guid roleId, Guid permissionId, Guid createdBy)
        {
            Id = Guid.NewGuid();
            RoleId = roleId;
            PermissionId = permissionId;
            CreatedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
        }
    }

}
