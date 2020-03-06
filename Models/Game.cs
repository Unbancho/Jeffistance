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
        private PlayerEventManager playerEventManager;
        private Player currentLeader;

        public bool InProgress = false;
        public List<Player> Players { get; private set; }
        public Phase CurrentPhase { get; set; } = Phase.Standby;
        public IEnumerable<Player> CurrentTeam { get; private set; }
        public Dictionary<int, int[]> TeamSizes { get; set; }
        public int NextTeamSize { get; private set; }
        public int CurrentRound { get; private set; }
        public IGamemode Gamemode { get; set; }

        public Game(IGamemode gm, PlayerEventManager pem)
        {
            Gamemode = gm;
            playerEventManager = pem;
            playerEventManager.OnTeamPicked += OnTeamPicked;
            TeamSizes = new Dictionary<int, int[]>()
            {
                {5, new int[] {2, 3, 2, 3, 3}}
            };
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
            CurrentRound = 0;
            NextTeamSize = TeamSizes[Players.Count()][CurrentRound];
        }

        private void PreparePlayers()
        {
            var i = 0;
            foreach (var player in Players)
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
            CurrentTeam = Players.Where((p) => args.PickedIDs.Contains(p.ID));
            CurrentPhase = Phase.TeamVoting;
        }
    }
}
