using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.UseCases.Permissions.Scan
{
    public interface IPermissionScanner
    {
        IEnumerable<PermissionScanResult> Scan();
    }
}
