using System;
using System.Collections.Generic;

namespace Jeffistance.Services
{
    public class PlayerEventManager
    {
        public delegate void TeamPickedHandler(TeamPickedArgs args);
        public event TeamPickedHandler OnTeamPicked;

        public delegate void TeamVotedHandler(VoteArgs args);
        public event TeamVotedHandler OnTeamVoted;

        public delegate void MissionVotedHandler(VoteArgs args);
        public event MissionVotedHandler OnMissionVoted;

        public void PickTeam(IEnumerable<int> pickedIDs)
        {
            OnTeamPicked?.Invoke(new TeamPickedArgs(pickedIDs));
        }

        public void VoteTeam(int voterID, bool vote)
        {
            OnTeamVoted?.Invoke(new VoteArgs(voterID, vote));
        }

        public void VoteMission(int voterID, bool vote)
        {
            OnMissionVoted?.Invoke(new VoteArgs(voterID, vote));
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

    public class VoteArgs : EventArgs
    {
        public int VoterID { get; private set; }
        public bool Vote { get; private set; }

        public VoteArgs(int voterID, bool vote)
        {
            VoterID = voterID;
            Vote = vote;
        }
    }
}
