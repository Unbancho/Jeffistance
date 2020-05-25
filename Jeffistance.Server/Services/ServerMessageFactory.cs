using System;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;

namespace Jeffistance.JeffServer.Services
{
    public interface IServerMessageFactory
    {
        Message MakeUpdateMessage();
    }

    public class ServerMessageFactory : IServerMessageFactory
    {
        public Message MakeUpdateMessage()
        {
            return new Message(flags: JeffistanceFlags.Update);
        }
    }
}
