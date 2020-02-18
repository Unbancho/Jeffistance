namespace Jeffistance.Models
{
    public interface IRole
    {
        string Name { get; }
        bool HasAbilities { get; }
    }

    public class DefaultRole : IRole
    {
        public string Name { get; } = "Default";
        public bool HasAbilities { get; } = false;
    }

    public class RoleFactory
    {
        private DefaultRole defaultRole;

        public IRole MakeDefault()
        {
            return defaultRole ??= new DefaultRole();
        }
    }
}
