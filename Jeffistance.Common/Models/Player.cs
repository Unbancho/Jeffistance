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

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Player);
        }

        public bool Equals(Player p)
        {
            if (Object.ReferenceEquals(p, null)) return false;
            return ID == p.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator ==(Player lp, Player rp)
        {
            if (Object.ReferenceEquals(lp, null))
            {
                if (Object.ReferenceEquals(rp, null))
                {
                    return true;
                }
                return false;
            }
            return lp.Equals(rp);
        }

        public static bool operator !=(Player lp, Player rp)
        {
            return !(lp == rp);
        }

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
