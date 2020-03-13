using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeffistance.Models
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
            List<int> taken = new List<int>();
            List<int> resistanceIDs = GetUniqueRandomInts(resistanceCount, 0, players.Count(), taken);
            List<int> spiesIDs = GetUniqueRandomInts(spiesCount, 0, players.Count(), taken);

            foreach (var player in players.Where(p => resistanceIDs.Contains(p.ID)))
            {
                player.Faction = FactionFactory.GetResistance();
            }

            foreach (var player in players.Where(p => spiesIDs.Contains(p.ID)))
            {
                player.Faction = FactionFactory.GetSpies();
            }
        }

        private List<int> GetUniqueRandomInts(int n, int from, int to, List<int> taken)
        {
            Random random = new Random();
            List<int> result = new List<int>();

            for (int i = 0; i < n; i++)
            {
                int r = random.Next(from, to);
                while(taken.Contains(r))
                {
                    r = random.Next(from, to);
                }
                taken.Add(r);
                result.Add(r);
            }

            return result;
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
