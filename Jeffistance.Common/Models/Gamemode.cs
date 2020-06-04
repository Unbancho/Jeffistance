using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeffistance.Common.Models
{
    public interface IGamemode
    {
        FactionFactory FactionFactory { get; set; }
        RoleFactory RoleFactory { get; set; }
        Dictionary<int, int[]> Factions { get; set; }
        void AssignFactions(IEnumerable<Player> players);
        void AssignRoles(IEnumerable<Player> players);
        Player PickLeader(IEnumerable<Player> players);
    }

    public class BasicGamemode : IGamemode
    {
        private int nextLeaderID = 0;

        public FactionFactory FactionFactory { get; set; } = new FactionFactory();
        public RoleFactory RoleFactory { get; set; } = new RoleFactory();
        public Dictionary<int, int[]> Factions { get; set; }

        public BasicGamemode()
        {
            Factions = new Dictionary<int, int[]>
            {
                {5, new int[] {3, 2}},
                {6, new int[] {4, 2}},
                {7, new int[] {4, 3}}
            };
        }

        public void AssignFactions(IEnumerable<Player> players)
        {
            int resistanceCount = Factions[players.Count()][0];
            int spiesCount = Factions[players.Count()][1];
            List<Player> playerList = new List<Player>(players);
            ShufflePlayers(playerList);

            foreach (var player in playerList.Select((p, idx) => (p, idx)))
            {
                player.p.Faction = player.idx < resistanceCount ?
                FactionFactory.GetResistance() : FactionFactory.GetSpies();
            }
        }

        private void ShufflePlayers(List<Player> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {  
                n--;  
                int k = rng.Next(n + 1);  
                Player p = list[k];  
                list[k] = list[n];  
                list[n] = p;  
            }  
        }

        public void AssignRoles(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.Role = RoleFactory.MakeDefault();
            }
        }

        public Player PickLeader(IEnumerable<Player> players)
        {
            foreach (var p in players)
            {
                p.IsLeader = false;
            }
            var leader = players.First((p) => p.ID == nextLeaderID);
            leader.IsLeader = true;
            nextLeaderID = (nextLeaderID < players.Count() - 1) ? nextLeaderID + 1 : 0;
            return leader;
        }
    }
}
