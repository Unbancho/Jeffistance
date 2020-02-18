using System.Collections.Generic;

namespace Jeffistance.Models
{
    public interface IGamemode
    {
        FactionFactory FactionFactory { get; set; }
        RoleFactory RoleFactory { get; set; }
        void AssignFactions(IEnumerable<Player> players);
        void AssignRoles(IEnumerable<Player> players);
        void PickLeader(IEnumerable<Player> players);
    }

    public class BasicGamemode : IGamemode
    {
        public FactionFactory FactionFactory { get; set; } = new FactionFactory();
        public RoleFactory RoleFactory { get; set; } = new RoleFactory();

        public void AssignFactions(IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                player.Faction = FactionFactory.MakeResistance();
            }
        }

        public void AssignRoles(IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                player.Role = RoleFactory.MakeDefault();
            }
        }

        public void PickLeader(IEnumerable<Player> players)
        {
            
        }
    }
}
