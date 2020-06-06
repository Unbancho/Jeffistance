using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Models;
using ModusOperandi.Networking;
using ModusOperandi.Messaging;

namespace Jeffistance.JeffServer.Services.MessageProcessing
{

    public class ServerMessageProcessor : JeffistanceMessageProcessor
    {
        Server Server {get;}
        public ServerMessageProcessor(Server server):base()
        {
            Server = server;
        }

        [MessageMethod(JeffistanceFlags.Greeting)]
        private void GreetingFlagMethod(Message message)
        {
            User user = (User) message["User"];
            ClientConnection connection = (ClientConnection) message.Sender;
            user.Connection = connection;
            Server.AddUser(user);
        }

        [MessageMethod(JeffistanceFlags.Chat)]
        private void ChatFlagMethod(Message message)
        {
            Server.Broadcast(message);
        }

        [MessageMethod(JeffistanceFlags.LobbyReady)]
        private void LobbyReadyFlagMethod(Message message)
        {
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
        }

        [MessageMethod(JeffistanceFlags.GetPlayerInfoMessage)]
        private void GetPlayerInfoMessageFlagMethod(Message message)
        {
            Server.Broadcast(message);
        }
        

    }
}