using System.Collections.Generic;
using Jeffistance.Common.Services.PlayerEventManager;
using System.Linq;
using System;

namespace Jeffistance.Common.Models
{
    public enum Phase
    {
        Standby,
        Setup,
        LeaderPicking,
        TeamPicking,
        TeamVoting,
        MissionVoting,
        GameEnd,
        ShowingTeamVoteResult
    }

    public class Game
    {
        private PlayerEventManager playerEventManager;

        public bool InProgress = false;
        public List<Player> Players {
            get => CurrentState.Players;
            private set => CurrentState.Players = value; }
        public Player CurrentLeader {
            get => CurrentState.CurrentLeader;
            private set => CurrentState.CurrentLeader = value; }
        public void StartTest()
        {
            InProgress = true;
            Setup();
            NextRound();
        }

        public Phase CurrentPhase {
            get => CurrentState.CurrentPhase;
            private set => CurrentState.CurrentPhase = value; }
        public IEnumerable<Player> CurrentTeam {
            get => CurrentState.CurrentTeam;
            private set => CurrentState.CurrentTeam = value; }
        public Dictionary<int, int[]> TeamSizes {
            get => CurrentState.TeamSizes;
            private set => CurrentState.TeamSizes = value; }
        public int NextTeamSize {
            get => CurrentState.NextTeamSize;
            private set => CurrentState.NextTeamSize = value; }
        public int CurrentRound {
            get => CurrentState.CurrentRound;
            private set => CurrentState.CurrentRound = value; }
        public int FailedVoteCount {
            get => CurrentState.FailedVoteCount;
            set => CurrentState.FailedVoteCount = value; }
        public int MaxFailedVotes { get; private set; } = 5;
        public int ResistanceWinCount {
            get => CurrentState.ResistanceWinCount;
            private set => CurrentState.ResistanceWinCount = value; }
        public int SpiesWinCount {
            get => CurrentState.SpiesWinCount;
            private set => CurrentState.SpiesWinCount = value; }
        public Dictionary<int, bool> CurrentTeamVotes { get; private set; }
        public Dictionary<int, bool> CurrentMissionVotes { get; private set; }
        public IGamemode Gamemode { get; set; }
        public GameState CurrentState { get; private set; }
        public Dictionary<int, string> leaderList { get; private set; }
        public int CurrentLeaderKey { get; private set; }
        public string CurrentLeaderID { get; private set; }
        public IFaction Winner {
            get => CurrentState.Winner;
            private set => CurrentState.Winner = value; }

        public Game(IGamemode gm, PlayerEventManager pem)
        {
            Gamemode = gm;
            playerEventManager = pem;
            CurrentState = new GameState();
            playerEventManager.OnTeamPicked += OnTeamPicked;
            playerEventManager.OnTeamVoted += OnTeamVoted;
            playerEventManager.OnMissionVoted += OnMissionVoted;
            TeamSizes = new Dictionary<int, int[]>()
            {
                {2, new int[] {1, 1, 1, 1, 1}},
                {3, new int[] {1, 2, 2, 3, 3}},
                {4, new int[] {2, 2, 2, 3, 3}},
                {5, new int[] {2, 3, 2, 3, 3}},
                {6, new int[] {2, 3, 4, 3, 4}},
                {7, new int[] {2, 3, 3, 4, 4}}
            };
            CurrentTeamVotes = new Dictionary<int, bool>();
            CurrentMissionVotes = new Dictionary<int, bool>();
            CurrentPhase = Phase.Standby;
        }

        public void Start(List<User> Users)
        {
            Players = new List<Player>();
            int i = 1;
            foreach(User u in Users)
            {
                Player player = new Player();
                player.ID = i;
                player.UserID = u.ID.ToString();
                i++;
                Players.Add(player);
            }
            
            InProgress = true;
            Setup();
            NextRound();
        }

        //Creates a shuffled dictionary of users whos key goes from 0 to the number of players. The list will loop back to 0 to pick leader when everyone was a leader already
        public void ShufflePlayersForLeader()
        {
            Random rng = new Random();
            var ll = Players.OrderBy(a => rng.Next());
            leaderList = new Dictionary<int, string>();
            CurrentLeaderKey = 0;
            int j = 0;
            foreach(Player p in ll)
            {
                leaderList.Add(j, p.UserID);
                j++;
            }
        }

        public void NextLeaderId()
        {
            CurrentPhase = Phase.LeaderPicking;
            if(CurrentLeaderKey == leaderList.Count)
            {
                CurrentLeaderKey = 0;
            }
            CurrentLeaderID = leaderList[CurrentLeaderKey];
            CurrentLeaderKey++;
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
            ShufflePlayersForLeader();
        }

        public void NextRound()
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
            FailedVoteCount = 0;
            NextLeaderId();
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
            Winner = (SpiesWinCount == 3 || FailedVoteCount == MaxFailedVotes) ?
            ff.GetSpies() : ff.GetResistance();
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
                FailedVoteCount++;
                if (FailedVoteCount == MaxFailedVotes)
                {
                    EndGame();
                    return;
                }
                CurrentTeamVotes.Clear();
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
