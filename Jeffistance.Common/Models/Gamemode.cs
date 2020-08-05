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
        void ShufflePlayersForFactions(List<Player> players);
        void ShufflePlayersForLeader(List<Player> players);
    }

    public class BasicGamemode : IGamemode
    {
        private int CurrentLeaderKey = 0;
        public Dictionary<int, Player> leaderList;
        public FactionFactory FactionFactory { get; set; } = new FactionFactory();
        public RoleFactory RoleFactory { get; set; } = new RoleFactory();
        public Dictionary<int, int[]> Factions { get; set; }

        public BasicGamemode()
        {
            Factions = new Dictionary<int, int[]>
            {
                {2, new int[] {1, 1}},
                {3, new int[] {2, 1}},
                {4, new int[] {3, 1}},
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
            ShufflePlayersForFactions(playerList);

            foreach (var player in playerList.Select((p, idx) => (p, idx)))
            {
                player.p.Faction = player.idx < resistanceCount ?
                FactionFactory.GetResistance() : FactionFactory.GetSpies();
            }
        }

       

        public void AssignRoles(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                player.Role = RoleFactory.MakeDefault();
            }
        }

        //Creates a shuffled dictionary of users whos key goes from 0 to the number of players. The list will loop back to 0 to pick leader when everyone was a leader already
        public void ShufflePlayersForLeader(List<Player> Players)
        {
            Random rng = new Random();
            var ll = Players.OrderBy(a => rng.Next());
            leaderList = new Dictionary<int, Player>();
            int j = 0;
            foreach(Player p in ll)
            {
                leaderList.Add(j, p);
                j++;
            }
        }

        public void ShufflePlayersForFactions(List<Player> list)
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

        public Player PickLeader(IEnumerable<Player> players)
        {
            if(CurrentLeaderKey == leaderList.Count)
            {
                CurrentLeaderKey = 0;
            }
            Player leader = leaderList[CurrentLeaderKey];
            CurrentLeaderKey++;
            return leader;
        }

      
    }
}
