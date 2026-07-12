using ProjectCore.Application.Common.Configuration;

namespace ProjectCore.Infrastructure.Configuration
{
    public sealed class AdminRoleSeedConfig : IAdminRoleSeedConfig
    {
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
    }
}
