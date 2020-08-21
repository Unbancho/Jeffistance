using System;
using System.Collections.Generic;
using System.Linq;
using Jeffistance.Common.Services.IoC;
using Jeffistance.JeffServer.Services;
using Jeffistance.Common.ExtensionMethods;

namespace Jeffistance.JeffServer.Models
{
    public class ServerLobby
    {
        private readonly Server _server;

        private List<Guid> _readyUserIDs;

        public ServerLobby(Server server)
        {
            _server = server;
            _readyUserIDs = new List<Guid>();
        }

        public void AddReadyUser(Guid userID)
        {
            string messageText = "";
            var user = _server.GetUser(userID);

            if (_readyUserIDs.Contains(userID))
            {
                _readyUserIDs.Remove(userID);
                messageText = $"{user.Name} is no longer ready.";
            }
            else
            {
                _readyUserIDs.Add(userID);
                messageText = $"{user.Name} is now ready.";
            }

            var chatManager = IoCManager.Resolve<IServerChatManager>();
            chatManager.Notify(messageText);
    
            CheckIfAllReady();
        }

        public void CheckIfAllReady()
        {
            bool ready = false;
            int lowerBound = _server.IsDedicated ? 2 : 1;
            int upperBound = _server.IsDedicated ? 7 : 6;

            if (_server.UserList.Where((u, i) => u.ID != _server.Host.ID)
                .All(user => _readyUserIDs.Contains(user.ID)) &&
                _readyUserIDs.Count.IsInRange(lowerBound, upperBound))
            {
                ready = true;
            }

            var messageFactory = IoCManager.Resolve<IServerMessageFactory>();
            var message = messageFactory.MakeEveryoneReadyStateMessage(ready);
            _server.Broadcast(message);

            if (ready)
            {
                var chatManager = IoCManager.Resolve<IServerChatManager>();
                chatManager.Notify("All players are now ready.");
                if (_server.IsDedicated) _server.StartCountdown();
            }
        }
    }
}
