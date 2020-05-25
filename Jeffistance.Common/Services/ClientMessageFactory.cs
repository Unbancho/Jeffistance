using System;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;
using Jeffistance.Common.Models;

namespace Jeffistance.Common.Services
{
    public interface IClientMessageFactory
    {
        Message MakeChatMessage(string text, string userID = null);
        Message MakeChatMessage(string text, Guid userID);

        Message MakeLobbyReadyMessage(string userID);
        Message MakeLobbyReadyMessage(Guid userID);

        Message MakeGreetingMessage(User user);

        Message MakeEditChatMessage(string content, string messageID);

        Message MakeDeleteChatMessage(string messageID);

        Message MakeJoinGameMessage();
    }

    public class ClientMessageFactory : IClientMessageFactory
    {
        public Message MakeChatMessage(string text, string userID = null)
        {
            var message = new Message(text, JeffistanceFlags.Chat);
            message["MessageID"] = Guid.NewGuid().ToString();
            if (userID != null)
            {
                message["UserID"] = userID;
            }

            return message;
        }

        public Message MakeChatMessage(string text, Guid userID)
        {
            return this.MakeChatMessage(text, userID.ToString());
        }

        public Message MakeLobbyReadyMessage(string userID)
        {
            var message = new Message(flags: JeffistanceFlags.LobbyReady);
            message["UserID"] = userID;
            return message;
        }

        public Message MakeLobbyReadyMessage(Guid userID)
        {
            return this.MakeLobbyReadyMessage(userID.ToString());
        }

        public Message MakeGreetingMessage(User user)
        {
            var message = new Message(flags: JeffistanceFlags.Greeting);
            message["User"] = user;
            return message;
        }

        public Message MakeEditChatMessage(string content, string messageID)
        {
            var message = new Message(flags: JeffistanceFlags.EditChatMessage);
            message["MessageID"] = messageID;
            message["NewText"] = content;
            return message;
        }

        public Message MakeDeleteChatMessage(string messageID)
        {
            var message = new Message(flags: JeffistanceFlags.DeleteChatMessage);
            message["MessageID"] = messageID;
            return message;
        }
        
        public Message MakeJoinGameMessage()
        {
            return new Message(flags: JeffistanceFlags.JoinGameMessage);
        }
    }
}
