using System;
using System.Collections.Generic;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;
using Jeffistance.Common.Models;

namespace Jeffistance.JeffServer.Services
{
    public interface IServerMessageFactory
    {
        Message MakeUpdateMessage();

        Message MakeChatMessage(string text);

        Message MakeEveryoneReadyStateMessage(bool ready);


        Message MakeJoinGameMessage();


        Message MakeGameStateUpdateMessage(GameState state);
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

        public Message MakeEveryoneReadyStateMessage(bool ready)
        {
            var message = new Message(flags: JeffistanceFlags.EveryoneReadyStateMessage);
            message["readyState"] = ready;
            return message;
        }

        public Message MakeJoinGameMessage()
        {
            return new Message(flags: JeffistanceFlags.JoinGameMessage);
        }

        public Message MakeGameStateUpdateMessage(GameState state)
        {
            var message = new Message(flags: JeffistanceFlags.GameStateUpdateMessage);
            message["GameState"] = state;
            return message;
        }
    }
}
