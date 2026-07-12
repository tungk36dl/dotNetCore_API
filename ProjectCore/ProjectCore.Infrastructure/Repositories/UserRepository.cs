using Microsoft.EntityFrameworkCore;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Interfaces;
using ProjectCore.Domain.Interfaces.UserRepository;
using ProjectCore.Domain.ValueObjects.User;
using ProjectCore.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<User?> GetByUserNameAsync(
            UserName userName,
            CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(
                    x => x.UserName.Value == userName.Value,
                    cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(
            Email email,
            CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(
                    x => x.Email.Value == email.Value,
                    cancellationToken);
        }

        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }

        public Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .Include(x => x.UserRoles)
                .FirstOrDefaultAsync(x => x.UserName.Value == userNameOrEmail || x.Email.Value == userNameOrEmail, cancellationToken);
        }

        public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(x => x.Email.Value == email.Value, cancellationToken);
        }

        public Task<bool> ExistsByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(x => x.UserName.Value == userName.Value, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(x => x.UserRoles)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<User> Data, int TotalCount)> GetDataAsync(
     UserSearch search,
     CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = _context.Users
                .Include(x => x.UserRoles)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search.UserName))
            {
                query = query.Where(x =>
                    x.UserName.Value.Contains(search.UserName));
            }

            if (!string.IsNullOrWhiteSpace(search.Email))
            {
                query = query.Where(x =>
                    x.Email.Value.Contains(search.Email));
            }

            if (!string.IsNullOrWhiteSpace(search.FullName))
            {
                query = query.Where(x =>
                    x.FullName != null &&
                    x.FullName.Value.Contains(search.FullName));
            }

            if (!string.IsNullOrWhiteSpace(search.Gender) &&
                Enum.TryParse<Gender>(search.Gender, true, out var gender))
            {
                query = query.Where(x => x.Gender == gender);
            }

            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                query = query.Where(x => x.UserName.Value.Contains(search.Keyword)
                                      || x.Email.Value.Contains(search.Keyword)
                                      || (x.FullName != null && x.FullName.Value.Contains(search.Keyword)));
            }

            if (search.RoleId.HasValue)
            {
                query = query.Where(x =>
                    x.UserRoles.Any(ur => ur.RoleId == search.RoleId));
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


        private static IQueryable<User> ApplySorting(
    IQueryable<User> query,
    SearchBase search)
        {
            if (string.IsNullOrWhiteSpace(search.SortBy))
                return query.OrderBy(x => x.UserName.Value);

            bool desc = search.SortDescending;

            return search.SortBy switch
            {
                "UserName" =>
                    desc ? query.OrderByDescending(x => x.UserName.Value)
                         : query.OrderBy(x => x.UserName.Value),

                "Email" =>
                    desc ? query.OrderByDescending(x => x.Email.Value)
                         : query.OrderBy(x => x.Email.Value),

                "FullName" =>
                    desc ? query.OrderByDescending(x => x.FullName!.Value)
                         : query.OrderBy(x => x.FullName!.Value),

                "Gender" =>
                    desc ? query.OrderByDescending(x => x.Gender)
                         : query.OrderBy(x => x.Gender),

                _ => query.OrderBy(x => x.UserName.Value)
            };
        }

    }

}
