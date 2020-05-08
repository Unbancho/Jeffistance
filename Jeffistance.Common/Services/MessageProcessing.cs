using System;
using System.Reflection;
using ModusOperandi.Messaging;
using ModusOperandi.Networking;
using System.Linq;
using System.Collections.Generic;

namespace Jeffistance.Common.Services.MessageProcessing
{

    [Flags]
    public enum JeffistanceFlags // Have to be powers of 2 for bitwise operations. Is the bitshifting way better or worse? Leave your comments down below!
    {
        Greeting = 1 << 0,
        Update = 1 << 2,
        Chat = 1 << 3
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

        public virtual void LogMessage(Message message)
        {
            Console.WriteLine(message.Text);
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
            ClientConnection?.Send(message);
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