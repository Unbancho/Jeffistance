using System.Collections.Generic;
using Jeffistance.Common.Services.PlayerEventManager;
using Jeffistance.Common.Services.IoC;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

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
        MissionVoteResult,
        FailedTeamFormation,
        AssigningRandomResult
    }

    public class Game
    {
        private PlayerEventManager playerEventManager;
        private FactionFactory _ff;

        public bool InProgress = false;
        public List<Player> Players {
            get => CurrentState.Players;
            private set => CurrentState.Players = value; }
        public Player CurrentLeader {
            get => CurrentState.CurrentLeader;
            private set => CurrentState.CurrentLeader = value; }

        public Phase CurrentPhase {
            get => CurrentState.CurrentPhase;
            set => CurrentState.CurrentPhase = value; }
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
            set => CurrentState.ResistanceWinCount = value; }
        public int SpiesWinCount {
            get => CurrentState.SpiesWinCount;
            set => CurrentState.SpiesWinCount = value; }
        public Dictionary<Player, bool> CurrentTeamVotes { get; private set; }
        public Dictionary<int, bool> CurrentMissionVotes { get; private set; }
        public IGamemode Gamemode { get; set; }
        public GameState CurrentState { get; private set; }
        public int CurrentLeaderKey { get; private set; }
        public IFaction Winner {
            get => CurrentState.Winner;
            private set => CurrentState.Winner = value; }
        
        public bool MissionVictory {
            get => CurrentState.MissionVictory;
            private set => CurrentState.MissionVictory = value;
        }

        public Game(IGamemode gm, PlayerEventManager pem)
        {
            Gamemode = gm;
            playerEventManager = pem;
            _ff = new FactionFactory();
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
            CurrentTeamVotes = new Dictionary<Player, bool>();
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
                player.Name = u.Name;
                i++;
                Players.Add(player);
            }
            
            InProgress = true;
            Setup();
        }

        private void Setup()
        {
            PreparePlayers();
            Gamemode.AssignFactions(Players);
            Gamemode.AssignRoles(Players);
            // -1 so first call to NextRound() sets it to 0
            CurrentRound = -1;
            ResistanceWinCount = 0;
            SpiesWinCount = 0;
            Gamemode.ShufflePlayersForLeader(Players);
            CurrentPhase = Phase.Setup;
        }

        public void NextRound(bool advanceRound)
        {
            if (ResistanceWinCount == 3 || SpiesWinCount == 3)
            {
                EndGame();
                return;
            }

            if (advanceRound)
            {
                CurrentRound++;
                NextTeamSize = TeamSizes[Players.Count()][CurrentRound];
                FailedVoteCount = 0;
            }

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
            DetermineWinner();
            CurrentPhase = Phase.GameEnd;
        }

        private void DetermineWinner()
        {
            Winner = (SpiesWinCount == 3 || FailedVoteCount == MaxFailedVotes) ?
            _ff.GetSpies() : _ff.GetResistance();
        }
        
        ///<summary>Rotates to the next leader updating the Game's CurrentLeader</summary>
        private void PickLeader()
        {
            CurrentLeader = Gamemode.PickLeader(Players);
            CurrentPhase = Phase.LeaderPicking;
        }

        private void PickTeam()
        {
            CurrentPhase = Phase.TeamPicking;
        }

        private void OnTeamPicked(TeamPickedArgs args)
        {
            CurrentTeam = Players.Where((p) => args.PickedIDs.Contains(p.ID)).ToList();
            CurrentTeamVotes.Clear();
            CurrentPhase = Phase.TeamVoting;
        }

        private void OnTeamVoted(VoteArgs args)
        {
            try
            {
                CurrentTeamVotes.Add(Players.Find(p => p.ID == args.VoterID), args.Vote);
            }
            catch (ArgumentException e)
            {
                var logger = IoCManager.GetServerLogger();
                logger.LogError(message: "Tried to add an already existing vote.", exception: e);
            }

            if (CurrentTeamVotes.Count() == Players.Count())
            {
                ResolveTeamVoting(CurrentTeamVotes.Values);
            }
        }

        private void ResolveTeamVoting(IEnumerable<bool> votes)
        {
            CurrentState.RevealedTeamVotes = CurrentTeamVotes;
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
                CurrentPhase = Phase.FailedTeamFormation;
            }
        }

        private void OnMissionVoted(VoteArgs args)
        {
            CurrentMissionVotes.Add(args.VoterID, args.Vote);
            if (CurrentMissionVotes.Count() == CurrentTeam.Count())
            {
                ResolveMissionVoting(CurrentMissionVotes.Values);
                // TODO implement a message queue in GameScreen to reduce ready messages
                CurrentPhase = Phase.MissionVoteResult;
            }
        }

        private void ResolveMissionVoting(IEnumerable<bool> votes)
        {
            if (votes.Any(vote => !vote))
            {
                SpiesWinCount++;
                MissionVictory = false;
            }
            else
            {
                ResistanceWinCount++;
                MissionVictory = true;
            }
        }
    }
}
