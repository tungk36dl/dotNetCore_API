using ProjectCore.Application.Common.Configuration;

namespace ProjectCore.Infrastructure.Configuration
{
    public sealed class AdminSeedConfig : IAdminSeedConfig
    {
        public string UserName { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string? FullName { get; init; }
    }
}
