using System.Collections.Generic;
using Jeffistance.Services;
using System.Linq;

namespace Jeffistance.Models
{
    public enum Phase
    {
        Standby,
        Setup,
        LeaderPicking,
        TeamPicking,
        TeamVoting
    }

    public class Game
    {
        private IGamemode gamemode;
        private PlayerEventManager playerEventManager;
        private Player currentLeader;
        private IEnumerable<Player> currentTeam;

        public bool InProgress = false;
        public List<Player> Players;
        public Phase CurrentPhase { get; set; } = Phase.Standby;
        public IEnumerable<Player> CurrentTeam { get => currentTeam; }
        public IGamemode Gamemode { get => gamemode; set => gamemode = value; }

        public Game(IGamemode gm, PlayerEventManager pem)
        {
            Gamemode = gm;
            playerEventManager = pem;
            playerEventManager.OnTeamPicked += OnTeamPicked;
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
            currentLeader = Gamemode.PickLeader(Players);
        }

        private void PickTeam()
        {
            CurrentPhase = Phase.TeamPicking;
        }

        private void OnTeamPicked(TeamPickedArgs args)
        {
            currentTeam = Players.Where((p) => args.PickedIDs.Contains(p.ID));
            CurrentPhase = Phase.TeamVoting;
        }
    }
}
