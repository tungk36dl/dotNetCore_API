using ProjectCore.Domain.Interfaces;

namespace ProjectCore.Domain.Interfaces.RoleRepository
{
    public class RoleSearch : SearchBase
    {
        public string? Name { get; set; }
    }
}
