using System.Collections.Generic;
using System.Linq;

namespace Jeffistance.Models
{
    public interface IGamemode
    {
        FactionFactory FactionFactory { get; set; }
        RoleFactory RoleFactory { get; set; }
        void AssignFactions(IEnumerable<Player> players);
        void AssignRoles(IEnumerable<Player> players);
        Player PickLeader(IEnumerable<Player> players);
    }

    public class BasicGamemode : IGamemode
    {
        private int nextLeaderID = 0;

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

        public Player PickLeader(IEnumerable<Player> players)
        {
            foreach (Player p in players)
            {
                p.IsLeader = false;
            }
            Player leader = players.First((p) => p.ID == nextLeaderID);
            leader.IsLeader = true;
            nextLeaderID = (nextLeaderID < players.Count() - 1) ? nextLeaderID + 1 : 0;
            return leader;
        }
    }
}
