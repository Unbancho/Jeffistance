using System;
using System.Reflection;
using ModusOperandi.Messaging;
using ModusOperandi.Networking;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Jeffistance.Common.Services.IoC;

namespace Jeffistance.Common.Services.MessageProcessing
{

    [Flags]
    public enum JeffistanceFlags // Have to be powers of 2 for bitwise operations. Is the bitshifting way better or worse? Leave your comments down below!
    {
        Greeting = 1 << 0,
        Update = 1 << 2,
        Chat = 1 << 3,
        LobbyReady = 1 << 4,
        EditChatMessage = 1 << 5,
        DeleteChatMessage = 1 << 6,
        JoinGameMessage = 1 << 7,
        GetPlayerInfoMessage = 1 << 8,
        GamePhaseReadyMessage = 1 << 9,
        AdvanceGamePhaseMessage = 1 << 10,
        PickTeamMessage = 1 << 11,
        DeclareLeaderMessage = 1 << 12,
        VoteMessage = 1 << 13,
        StartMissionVotingMessage = 1 << 14,
        ShowTeamVoteResultMessage = 1 << 15,
        MissionVoteMessage = 1 << 16,
        ShowMissionResultMessage = 1 << 17,
        EndGameMessage = 1 << 18,
        EveryoneReadyStateMessage = 1 << 19
    }

    public class JeffistanceMessageProcessor : MessageProcessor<JeffistanceFlags>
    {
        public override Dictionary<JeffistanceFlags, MethodInfo> ProcessingMethods {get; set;}

        public JeffistanceMessageProcessor()
        {
            ProcessingMethods = new Dictionary<JeffistanceFlags, MethodInfo>();
            foreach (var processingMethod in GetType().GetMethods(BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(y => y.GetCustomAttributes().OfType<MessageMethodAttribute>().Any()))
            {
                var attr = (MessageMethodAttribute) processingMethod.GetCustomAttribute(typeof(MessageMethodAttribute));
                ProcessingMethods[attr.Flag] = processingMethod;
            }
        }

        public void LogMessage(ILogger logger, Message message)
        {
            string messageToLog = "Received network message:\n";
            if (message.Sender != null)
            {
                messageToLog += $"Sender: {((ConnectionTcp) message.Sender).SERVER_IP}\n";
            }
            else
            {
                messageToLog += "Sender: UNKNOWN\n";
            }
            messageToLog += $"Text: {message.Text}\nContents:";

            foreach (KeyValuePair<string, object> entry in message.PackedObjects)
            {
                var result = (obj: message.UnpackObject(entry.Key), name: entry.Key);
                (object obj, string name) = (ValueTuple<object, string>) result;
                messageToLog += $" {name}";
            }
            messageToLog += "\nFlags:";

            foreach (var flag in message.GetFlags())
            {
                messageToLog += $" {Enum.GetName(typeof(JeffistanceFlags), flag)}";
            }

            logger.LogDebug(messageToLog);
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MessageMethodAttribute : Attribute
    {
        public JeffistanceFlags Flag;

        public MessageMethodAttribute(JeffistanceFlags flag)
        {
            Flag = flag;
        }
    }

    public class MessageHandler
    {
        JeffistanceMessageProcessor Processor;
        ClientConnection ClientConnection {get;}
        ServerConnection ServerConnection {get;}

        public MessageHandler(JeffistanceMessageProcessor messageProcessor, ClientConnection clientConnection)
        {
            Processor = messageProcessor;
            ClientConnection = clientConnection;
        }

        public MessageHandler(JeffistanceMessageProcessor messageProcessor, ServerConnection serverConnection)
        {
            Processor = messageProcessor;
            ServerConnection = serverConnection;
        }

        public void Broadcast(Message message)
        {
           ServerConnection?.Broadcast(message);
        }

        public void Broadcast(string message)
        {
            Broadcast(new Message(message));
        }

        public void Send(Message message)
        {
            try
            {
                ClientConnection?.Send(message);
            }
            catch (ObjectDisposedException e)
            {
                var logger = IoCManager.GetClientLogger();
                logger.LogError(message: "Message failed to send.", exception: e);
            }
        }

        public void Send(string message)
        {
            Send(new Message(message));
        }

        public void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            var message = (Message) args.Message;
            try
            {
                message.Sender = args.Sender;
                Processor.ProcessMessage(message);
            }
            catch(NullReferenceException){}
        }
    }
}   