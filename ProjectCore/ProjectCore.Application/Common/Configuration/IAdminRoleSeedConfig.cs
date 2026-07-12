using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCore.Application.Common.Configuration
{
    public interface IAdminRoleSeedConfig
    {
        string Name { get; }
        string? Description { get; }
    }
}
