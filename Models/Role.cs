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
        private DefaultRole _defaultRole;

        public IRole MakeDefault()
        {
            return _defaultRole ??= new DefaultRole();
        }
    }
}
