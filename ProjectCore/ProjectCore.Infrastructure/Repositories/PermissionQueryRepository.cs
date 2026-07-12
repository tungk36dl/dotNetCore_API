using Microsoft.EntityFrameworkCore;
using ProjectCore.Application.Interfaces;
using ProjectCore.Domain.ValueObjects.Permission;
using ProjectCore.Infrastructure.Persistence;

namespace ProjectCore.Infrastructure.Repositories
{
    public sealed class PermissionQueryRepository : IPermissionQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PermissionCode>> GetPermissionsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var codeValues = await (
                from ur in _context.UserRoles.AsNoTracking()
                join rp in _context.RolePermissions.AsNoTracking() on ur.RoleId equals rp.RoleId
                join p in _context.Permissions.AsNoTracking() on rp.PermissionId equals p.Id
                where ur.UserId == userId
                select p.Code.Value
            )
            .Distinct()
            .ToListAsync(cancellationToken);

            return codeValues.Select(v => new PermissionCode(v)).ToList();
        }
    }

}
