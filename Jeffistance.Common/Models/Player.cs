using System;
using System.Runtime.Serialization;

namespace Jeffistance.Common.Models
{
    [Serializable]
    public class Player : ISerializable
    {
        public IFaction Faction { get; set; }
        public IRole Role { get; set; }
        public int ID { get; set; } = -1;
        public string Name { get; set; }
        public bool IsLeader { get; set; } = false;
        public string UserID { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Faction", Faction, typeof(IFaction)); 
            info.AddValue("Role", Role, Role.GetType());
            info.AddValue("ID", ID);
            info.AddValue("Name", Name);
            info.AddValue("IsLeader", IsLeader);
            info.AddValue("UserID", UserID);
        }

        public Player(){}

        protected Player(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt32("ID");
            Name = info.GetString("Name");
            IsLeader = info.GetBoolean("IsLeader");
            UserID = info.GetString("UserID");
            Role = (IRole) info.GetValue("Role", typeof(DefaultRole));
            Faction = (IFaction) info.GetValue("Faction", typeof(IFaction));
        }
    }
}
