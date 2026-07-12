using ProjectCore.Domain.Entities;
using ProjectCore.Domain.ValueObjects.Role;

namespace ProjectCore.Domain.Entities
{

    public class Role : DomainEntity<Guid>
    {
        public RoleName Name { get; private set; }
        public string? Description { get; private set; }

        private readonly List<RolePermission> _rolePermissions = new();
        public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

        protected Role() { }

        public Role(Guid id, RoleName name, Guid createdBy)
            : base(id, createdBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public Role(Guid id, RoleName name, string? description, Guid createdBy)
       : base(id, createdBy)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
        }

        public void UpdateDetails(string? name, string? description, Guid updatedBy)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = new RoleName(name);
            }
            if (description is not null)
                Description = description;
            MarkUpdated(updatedBy);
        }

        public void AddPermission(Guid permissionId, Guid createdBy)
        {
            if (_rolePermissions.Any(x => x.PermissionId == permissionId))
                return;

            _rolePermissions.Add(new RolePermission(Id, permissionId, createdBy));
        }

        public void RemovePermission(Guid permissionId)
        {
            var permission = _rolePermissions.FirstOrDefault(x => x.PermissionId == permissionId);
            if (permission != null)
                _rolePermissions.Remove(permission);
        }
    }

}

