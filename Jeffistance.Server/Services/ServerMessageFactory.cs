using System;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;

namespace Jeffistance.JeffServer.Services
{
    public interface IServerMessageFactory
    {
        Message MakeUpdateMessage();

        Message MakeChatMessage(string text);
    }

    public class ServerMessageFactory : IServerMessageFactory
    {
        public Message MakeUpdateMessage()
        {
            return new Message(flags: JeffistanceFlags.Update);
        }

        public Message MakeChatMessage(string text)
        {
            var message = new Message(text, JeffistanceFlags.Chat);
            message["MessageID"] = Guid.NewGuid().ToString();
            return message;
        }
    }
}
