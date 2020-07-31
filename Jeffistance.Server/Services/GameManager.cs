using System;
using System.Collections.Generic;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Models;

namespace Jeffistance.JeffServer.Services
{
    public class GameManager
    {
        private readonly Server _server;

        private readonly Game _game;

        private readonly IServerMessageFactory _messageFactory;

        private List<Guid> _readyUserIDs;

        public GameManager(Server server, Game game, IServerMessageFactory messageFactory)
        {
            _server = server;
            _game = game;
            _messageFactory = messageFactory;
            _readyUserIDs = new List<Guid>();
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
                case Phase.Standby:
                    var message = _messageFactory.MakeDeclareLeaderMessage(
                        _game.NextTeamSize, _game.CurrentLeader);
                    _server.Broadcast(message);
                    break;
                
                case Phase.AssigningRandomResult:
                    
                    break;
            }
        }
    }
}
