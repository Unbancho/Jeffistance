using System;
using System.Collections.Generic;

namespace Jeffistance.Services
{
    public class PlayerEventManager
    {
        public delegate void TeamPickedHandler(TeamPickedArgs args);
        public event TeamPickedHandler OnTeamPicked;

        public void PickTeam(IEnumerable<int> pickedIDs)
        {
            OnTeamPicked?.Invoke(new TeamPickedArgs(pickedIDs));
        }
    }

    public class TeamPickedArgs : EventArgs
    {
        private IEnumerable<int> pickedIDs;

        public IEnumerable<int> PickedIDs { get => pickedIDs; }

        public TeamPickedArgs(IEnumerable<int> pickedIDs)
        {
            this.pickedIDs = pickedIDs;
        }
    }
}
