using System;
using System.Collections.Generic;
using System.ComponentModel;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services.PlayerEventManager;
using Jeffistance.JeffServer.Models;

namespace Jeffistance.JeffServer.Services
{
    public class GameManager
    {
        private readonly Server _server;

        private readonly Game _game;

        private readonly IServerMessageFactory _messageFactory;

        private readonly PlayerEventManager _pem;

        private List<Guid> _readyUserIDs;

        public GameManager(Server server, Game game,
            IServerMessageFactory messageFactory, PlayerEventManager pem)
        {
            _server = server;
            _game = game;
            _messageFactory = messageFactory;
            _pem = pem;
            _readyUserIDs = new List<Guid>();
            _game.CurrentState.PropertyChanged += OnGameStateUpdate;
        }

        public void Start(List<User> Users)
        {
            _game.Start(Users);
        }

        public void AddReadyUser(Guid userID)
        {
            if (!_readyUserIDs.Contains(userID))
            {
                _readyUserIDs.Add(userID);
            }
            CheckIfAllReady();
        }

        private void CheckIfAllReady()
        {
            if (_server.UserList.Count == _readyUserIDs.Count)
            {
                AdvanceGamePhase();
                _readyUserIDs.Clear();
            }
        }

        private void AdvanceGamePhase()
        {
            switch (_game.CurrentPhase)
            {
                case Phase.Setup:
                    _game.NextRound(true);
                    break;
                
                case Phase.FailedTeamFormation:
                    _game.NextRound(false);
                    break;
                
                case Phase.MissionVoteResult:
                    _game.NextRound(true);
                    break;
            }
        }

        private void OnGameStateUpdate(object sender, PropertyChangedEventArgs args)
        {
            var message = _messageFactory.MakeGameStateUpdateMessage(_game.CurrentState);
            _server.Broadcast(message);
        }

        public void OnTeamPicked(List<string> pickedUserIDs)
        {
            List<int> ids = new List<int>();
            foreach (string userID in pickedUserIDs)
            {
                var player = GetPlayerByUserId(userID);
                ids.Add(player.ID);
            }
            _pem.PickTeam(ids);
        }

        public void OnTeamVoted(int id, bool vote)
        {
            _pem.VoteTeam(id, vote);
        }

        public void OnMissionVoted(int id, bool vote)
        {
            _pem.VoteMission(id, vote);
        }

        private Player GetPlayerByUserId(string userID)
        {
            return _game.Players.Find((p) => p.UserID == userID);
        }
    }
}
