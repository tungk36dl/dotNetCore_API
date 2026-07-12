using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.UseCases.Permissions.Scan
{
    public sealed class PermissionScanResult
    {
        public string Module { get; init; } = default!;
        public string Action { get; init; } = default!;
    }

}
