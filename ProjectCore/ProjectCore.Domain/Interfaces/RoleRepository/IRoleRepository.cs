using ProjectCore.Domain.Entities;
using ProjectCore.Domain.ValueObjects.Role;

namespace ProjectCore.Domain.Interfaces.RoleRepository
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
        Task<Role?> GetByNameAsync(
            RoleName roleName,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(
            RoleName roleName,
            CancellationToken cancellationToken = default);
        Task AddAsync(Role role, CancellationToken cancellationToken = default);
        void Update(Role role);
        void Remove(Role role);
        Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<Role> Data, int TotalCount)> GetDataAsync(RoleSearch search, CancellationToken cancellationToken = default);
    }
}
