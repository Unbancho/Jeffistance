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
        MissionVoting,
        GameEnd
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
        public int MaxRound { get; private set; } = 4;
        public int FailureCount { get; private set; }
        public int ResistanceWinCount { get; private set; }
        public int SpiesWinCount { get; private set; }
        public Dictionary<int, bool> CurrentTeamVotes { get; private set; }
        public Dictionary<int, bool> CurrentMissionVotes { get; private set; }
        public IGamemode Gamemode { get; set; }
        public IFaction Winner { get; private set; }

        public Game(IGamemode gm, PlayerEventManager pem)
        {
            Gamemode = gm;
            playerEventManager = pem;
            playerEventManager.OnTeamPicked += OnTeamPicked;
            playerEventManager.OnTeamVoted += OnTeamVoted;
            playerEventManager.OnMissionVoted += OnMissionVoted;
            TeamSizes = new Dictionary<int, int[]>()
            {
                {5, new int[] {2, 3, 2, 3, 3}},
                {6, new int[] {2, 3, 4, 3, 4}}
            };
            CurrentTeamVotes = new Dictionary<int, bool>();
            CurrentMissionVotes = new Dictionary<int, bool>();
        }

        public void Start(IEnumerable<Player> players)
        {
            InProgress = true;
            Players = new List<Player>(players);
            Setup();
            NextRound();
        }

        private void Setup()
        {
            CurrentPhase = Phase.Setup;
            PreparePlayers();
            Gamemode.AssignFactions(Players);
            Gamemode.AssignRoles(Players);
            // -1 so first call to NextRound() sets it to 0
            CurrentRound = -1;
            ResistanceWinCount = 0;
            SpiesWinCount = 0;
        }

        private void NextRound()
        {
            if (ResistanceWinCount == 3 || SpiesWinCount == 3)
            {
                EndGame();
                return;
            }
            CurrentRound++;
            NextTeamSize = TeamSizes[Players.Count()][CurrentRound];
            CurrentTeamVotes.Clear();
            CurrentMissionVotes.Clear();
            PickLeader();
            PickTeam();
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

        private void EndGame()
        {
            CurrentPhase = Phase.GameEnd;
            DetermineWinner();
        }

        private void DetermineWinner()
        {
            FactionFactory ff = new FactionFactory();
            Winner = (ResistanceWinCount == 3) ? ff.GetResistance() : ff.GetSpies();
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

        private void OnTeamVoted(VoteArgs args)
        {
            CurrentTeamVotes.Add(args.VoterID, args.Vote);
            if (CurrentTeamVotes.Count() == Players.Count())
            {
                ResolveTeamVoting(CurrentTeamVotes.Values);
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

        private void OnMissionVoted(VoteArgs args)
        {
            CurrentMissionVotes.Add(args.VoterID, args.Vote);
            if (CurrentMissionVotes.Count() == CurrentTeam.Count())
            {
                ResolveMissionVoting(CurrentMissionVotes.Values);
                NextRound();
            }
        }

        private void ResolveMissionVoting(IEnumerable<bool> votes)
        {
            if (votes.Any(vote => !vote))
            {
                SpiesWinCount++;
            }
            else
            {
                ResistanceWinCount++;
            }
        }
    }
}
