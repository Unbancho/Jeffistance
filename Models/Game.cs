using System.Collections.Generic;

namespace Jeffistance.Models
{
    public enum Phase
    {
        Standby,
        Setup,
        LeaderPicking,
        TeamPicking
    }

    public class Game
    {
        private IGamemode gamemode;

        public bool InProgress = false;
        public List<Player> Players;
        public Phase CurrentPhase { get; set; } = Phase.Standby;
        public IGamemode Gamemode { get => gamemode; set => gamemode = value; }

        public Game(IGamemode gm)
        {
            Gamemode = gm;
        }

        public void Start(IEnumerable<Player> players)
        {
            InProgress = true;
            Players = new List<Player>(players);
            Setup();
            PickLeader();
            PickTeam();
        }

        private void Setup()
        {
            CurrentPhase = Phase.Setup;
            PreparePlayers();
            Gamemode.AssignFactions(Players);
            Gamemode.AssignRoles(Players);
        }

        private void PreparePlayers()
        {
            int i = 0;
            foreach (Player player in Players)
            {
                player.ID = i;
                i++;
            }
        }

        private void PickLeader()
        {
            CurrentPhase = Phase.LeaderPicking;
            Gamemode.PickLeader(Players);
        }

        private void PickTeam()
        {
            CurrentPhase = Phase.TeamPicking;
        }
    }
}
