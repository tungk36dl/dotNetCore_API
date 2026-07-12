using Microsoft.EntityFrameworkCore;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Interfaces;
using ProjectCore.Domain.Interfaces.RoleRepository;
using ProjectCore.Domain.ValueObjects.Role;
using ProjectCore.Infrastructure.Persistence;

namespace ProjectCore.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(x => x.RolePermissions)
                .FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
        }

        public async Task<Role?> GetByNameAsync(RoleName roleName, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(x => x.RolePermissions)
                .FirstOrDefaultAsync(x => x.Name.Value == roleName.Value, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(RoleName roleName, CancellationToken cancellationToken = default)
        {
            return await _context.Roles.AnyAsync(x => x.Name.Value == roleName.Value, cancellationToken);
        }

        public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
        {
            await _context.Roles.AddAsync(role, cancellationToken);
        }

        public void Update(Role role)
        {
            _context.Roles.Update(role);
        }

        public void Remove(Role role)
        {
            _context.Roles.Remove(role);
        }

        public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(x => x.RolePermissions)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Role> Data, int TotalCount)> GetDataAsync(
            RoleSearch search,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Role> query = _context.Roles
                .Include(x => x.RolePermissions)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                query = query.Where(x =>
                    x.Name.Value.Contains(search.Name));
            }

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                query = query.Where(x => 
                    x.Name.Value.Contains(search.Keyword) ||
                    (x.Description != null && x.Description.Contains(search.Keyword)));
            }

            query = ApplySorting(query, search);

            var totalCount = await query.CountAsync(cancellationToken);

            int skip = (search.Page - 1) * search.PageSize;

            query = query
                .Skip(skip)
                .Take(search.PageSize);

            var data = await query.ToListAsync(cancellationToken);
            return (data, totalCount);
        }

        private static IQueryable<Role> ApplySorting(
            IQueryable<Role> query,
            SearchBase search)
        {
            if (string.IsNullOrWhiteSpace(search.SortBy))
                return query.OrderBy(x => x.Name.Value);

            bool desc = search.SortDescending;

            return search.SortBy switch
            {
                "Name" =>
                    desc ? query.OrderByDescending(x => x.Name.Value)
                         : query.OrderBy(x => x.Name.Value),

                "Description" =>
                    desc ? query.OrderByDescending(x => x.Description ?? string.Empty)
                         : query.OrderBy(x => x.Description ?? string.Empty),

                "CreatedDate" =>
                    desc ? query.OrderByDescending(x => x.CreatedDate)
                         : query.OrderBy(x => x.CreatedDate),

                _ => query.OrderBy(x => x.Name.Value)
            };
        }
    }
}
