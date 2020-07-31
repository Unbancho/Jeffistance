using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Jeffistance.Common.Models
{
    [Serializable]
    public class GameState : ISerializable
    {
        public List<Player> Players { get; set; }
        public Phase CurrentPhase { get; set; }
        public Player CurrentLeader { get; set; }
        public IEnumerable<Player> CurrentTeam { get; set; }
        public Dictionary<int, int[]> TeamSizes { get; set; }
        public int NextTeamSize { get; set; }
        public int CurrentRound { get; set; }
        public int FailedVoteCount { get; set; }
        public int ResistanceWinCount { get; set; }
        public int SpiesWinCount { get; set; }
        public IFaction Winner { get; set; }
        public Dictionary<int, bool> RevealedTeamVotes { get; set; }
        public Dictionary<string, bool> TeamVote { get; set; }

        public GameState()
        {
            RevealedTeamVotes = new Dictionary<int, bool>();
            TeamVote = new Dictionary<string, bool>();
        }

        public GameState(SerializationInfo info, StreamingContext context)
        {
            Players = (List<Player>) info.GetValue("Players", typeof(List<Player>));
            CurrentPhase = (Phase) info.GetValue("CurrentPhase", typeof(Phase));
            CurrentLeader = (Player) info.GetValue("CurrentLeader", typeof(Player));
            CurrentTeam = (IEnumerable<Player>) info.GetValue("CurrentTeam", typeof(IEnumerable<Player>));
            TeamSizes = (Dictionary<int, int[]>) info.GetValue("TeamSizes", typeof(Dictionary<int, int[]>));
            NextTeamSize = info.GetInt32("NextTeamSize");
            CurrentRound = info.GetInt32("CurrentRound");
            FailedVoteCount = info.GetInt32("FailedVoteCount");
            ResistanceWinCount = info.GetInt32("ResistanceWinCount");
            SpiesWinCount = info.GetInt32("SpiesWinCount");
            Winner = (IFaction) info.GetValue("Winner", typeof(IFaction));
            RevealedTeamVotes = (Dictionary<int, bool>) info.GetValue("RevealedTeamVotes", typeof(Dictionary<int, bool>));
            TeamVote = (Dictionary<string, bool>) info.GetValue("TeamVote", typeof(Dictionary<string, bool>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Players", Players, typeof(List<Player>));
            info.AddValue("CurrentPhase", CurrentPhase, typeof(Phase));
            info.AddValue("CurrentLeader", CurrentLeader, typeof(Player));
            info.AddValue("CurrentTeam", CurrentTeam, typeof(IEnumerable<Player>));
            info.AddValue("TeamSizes", TeamSizes, typeof(Dictionary<int, int[]>));
            info.AddValue("NextTeamSize", NextTeamSize);
            info.AddValue("CurrentRound", CurrentRound);
            info.AddValue("FailedVoteCount", FailedVoteCount);
            info.AddValue("ResistanceWinCount", ResistanceWinCount);
            info.AddValue("SpiesWinCount", SpiesWinCount);
            info.AddValue("Winner", Winner, typeof(IFaction));
            info.AddValue("RevealedTeamVotes", RevealedTeamVotes, typeof(Dictionary<int, bool>));
            info.AddValue("TeamVote", TeamVote, typeof(Dictionary<string, bool>));
        }
    }
}
