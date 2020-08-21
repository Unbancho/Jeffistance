using System;
using System.Collections.Generic;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Models;
using ModusOperandi.Networking;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.IoC;

namespace Jeffistance.JeffServer.Services.MessageProcessing
{

    public class ServerMessageProcessor : JeffistanceMessageProcessor
    {
        Server Server {get;}
        public ServerMessageProcessor(Server server):base()
        {
            Server = server;
        }

        public override void ProcessMessage(Message message)
        {
            var logger = IoCManager.GetServerLogger();
            LogMessage(logger, message);

            base.ProcessMessage(message);
        }

        [MessageMethod(JeffistanceFlags.Greeting)]
        private void GreetingFlagMethod(Message message)
        {
            User user = (User) message["User"];
            ClientConnection connection = (ClientConnection) message.Sender;
            user.Connection = connection;
            Server.AddUser(user);
            Server.ChatManager.Notify($"{user.Name} has joined.");
        }

        [MessageMethod(JeffistanceFlags.Chat)]
        private void ChatFlagMethod(Message message)
        {
            Server.Broadcast(message);
        }

        [MessageMethod(JeffistanceFlags.LobbyReady)]
        private void LobbyReadyFlagMethod(Message message)
        {
            Server.Lobby.AddReadyUser(Guid.Parse((string) message["UserID"]));
            Server.Broadcast(message);
        }

        [MessageMethod(JeffistanceFlags.EditChatMessage)]
        private void EditChatMessageFlagMethod(Message message)
        {
            Server.Broadcast(message);
        }
        
        [MessageMethod(JeffistanceFlags.DeleteChatMessage)]
        private void DeleteChatMessageFlagMethod(Message message)
        {
            Server.Broadcast(message);
        }

        [MessageMethod(JeffistanceFlags.JoinGameMessage)]
        private void JoinGameMessageFlagMethod(Message message)
        {
            Server.Broadcast(message);
            Server.StartGame();
        }

        [MessageMethod(JeffistanceFlags.GamePhaseReadyMessage)]
        private void GamePhaseReadyMessageFlagMethod(Message message)
        {
            Server.GameManager.AddReadyUser(Guid.Parse((string) message["UserID"]));
        }


        [MessageMethod(JeffistanceFlags.PickTeamMessage)]
        private void PickTeamMessageFlagMethod(Message message)
        {
            Server.GameManager.OnTeamPicked((List<string>) message["PlayersInTeamIDs"]);
        }

        [MessageMethod(JeffistanceFlags.VoteMessage)]
        private void VoteMessageFlagMethod(Message message)
        {
            Server.GameManager.OnTeamVoted((int) message["PlayerID"], (bool) message["Vote"]);
        }
        
        [MessageMethod(JeffistanceFlags.MissionVoteMessage)]
        private void MissionVoteMessageFlagMethod(Message message)
        {
            Server.GameManager.OnMissionVoted((int) message["PlayerID"], (bool) message["Vote"]);
        }
    }
}
