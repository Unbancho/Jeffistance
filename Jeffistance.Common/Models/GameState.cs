using System.Collections.Generic;

namespace Jeffistance.Common.Models
{
    public class GameState
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

        public GameState()
        {
            RevealedTeamVotes = new Dictionary<int, bool>();
        }
    }
}
