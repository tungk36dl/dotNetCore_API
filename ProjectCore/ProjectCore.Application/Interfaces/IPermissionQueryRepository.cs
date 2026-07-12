using ProjectCore.Domain.ValueObjects.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.Interfaces
{
    public interface IPermissionQueryRepository
    {
        Task<IReadOnlyList<PermissionCode>> GetPermissionsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken);
    }
}
