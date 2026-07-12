namespace ProjectCore.Application.Common.Configuration
{
    public interface IAdminSeedConfig
    {
        string UserName { get; }
        string Email { get; }
        string Password { get; }
        string? FullName { get; }
    }
}
