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
        TeamVoting,
        MissionVoting
    }

    public class Game
    {
        private PlayerEventManager playerEventManager;

        public bool InProgress = false;
        public List<Player> Players { get; private set; }
        public Player CurrentLeader { get; private set; }
        public Phase CurrentPhase { get; set; } = Phase.Standby;
        public IEnumerable<Player> CurrentTeam { get; private set; }
        public Dictionary<int, int[]> TeamSizes { get; set; }
        public int NextTeamSize { get; private set; }
        public int CurrentRound { get; private set; }
        public int FailureCount { get; private set; }
        public Dictionary<int, bool> CurrentVotes { get; private set; }
        public IGamemode Gamemode { get; set; }

        public Game(IGamemode gm, PlayerEventManager pem)
        {
            Gamemode = gm;
            playerEventManager = pem;
            playerEventManager.OnTeamPicked += OnTeamPicked;
            playerEventManager.OnTeamVoted += OnTeamVoted;
            TeamSizes = new Dictionary<int, int[]>()
            {
                {5, new int[] {2, 3, 2, 3, 3}},
                {6, new int[] {2, 3, 4, 3, 4}}
            };
            CurrentVotes = new Dictionary<int, bool>();
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
            CurrentLeader = Gamemode.PickLeader(Players);
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

        private void OnTeamVoted(TeamVotedArgs args)
        {
            CurrentVotes.Add(args.VoterID, args.Vote);
            if (CurrentVotes.Count() == Players.Count())
            {
                ResolveTeamVoting(CurrentVotes.Values);
            }
        }

        private void ResolveTeamVoting(IEnumerable<bool> votes)
        {
            int acceptedVotesCount = votes.Count(vote => vote);
            int rejectedVotesCount = votes.Count(vote => !vote);
            if (acceptedVotesCount > rejectedVotesCount)
            {
                CurrentPhase = Phase.MissionVoting;
            }
            else
            {
                FailureCount++;
                PickLeader();
                PickTeam();
            }
        }
    }
}
