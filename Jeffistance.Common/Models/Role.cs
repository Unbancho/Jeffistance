using System;
using System.Runtime.Serialization;

namespace Jeffistance.Common.Models
{
    public interface IRole
    {
        string Name { get; }
        bool HasAbilities { get; }
    }

    [Serializable]
    public class DefaultRole : IRole, ISerializable
    {
        public string Name { get; } = "Default";
        public bool HasAbilities { get; } = false;

        public DefaultRole(){}
        protected DefaultRole(SerializationInfo info, StreamingContext context)
        {
            HasAbilities = info.GetBoolean("HasAbilities");
            Name = info.GetString("Name");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("HasAbilities", HasAbilities);
        }
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
