using Microsoft.EntityFrameworkCore;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Interfaces.PermissionRepository;
using ProjectCore.Infrastructure.Persistence;

namespace ProjectCore.Infrastructure.Repositories
{
    public sealed class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string module, string action, CancellationToken ct)
        {
            return await _context.Permissions
                .AnyAsync(p => p.Module == module && p.Action == action, ct);
        }

        public async Task AddAsync(Permission permission, CancellationToken ct)
        {
            await _context.Permissions.AddAsync(permission, ct);
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Permissions
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var idList = ids.ToList();
            return await _context.Permissions
                .AsNoTracking()
                .Where(p => idList.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(ct);
        }
    }
}

