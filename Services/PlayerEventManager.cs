using System;
using System.Collections.Generic;

namespace Jeffistance.Services
{
    public class PlayerEventManager
    {
        public delegate void TeamPickedHandler(TeamPickedArgs args);
        public event TeamPickedHandler OnTeamPicked;

        public delegate void TeamVotedHandler(TeamVotedArgs args);
        public event TeamVotedHandler OnTeamVoted;

        public void PickTeam(IEnumerable<int> pickedIDs)
        {
            OnTeamPicked?.Invoke(new TeamPickedArgs(pickedIDs));
        }

        public void VoteTeam(int voterID, bool vote)
        {
            OnTeamVoted?.Invoke(new TeamVotedArgs(voterID, vote));
        }
    }

    public class TeamPickedArgs : EventArgs
    {

        public IEnumerable<int> PickedIDs { get; private set; }

        public TeamPickedArgs(IEnumerable<int> pickedIDs)
        {
            PickedIDs = pickedIDs;
        }
    }

    public class TeamVotedArgs : EventArgs
    {
        public int VoterID { get; private set; }
        public bool Vote { get; private set; }

        public TeamVotedArgs(int voterID, bool vote)
        {
            VoterID = voterID;
            Vote = vote;
        }
    }
}
