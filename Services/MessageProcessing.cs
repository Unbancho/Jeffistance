using System;
using System.Reflection;
using Jeffistance.Models;
using ModusOperandi.Messaging;
using ModusOperandi.Networking;
using System.Linq;
using System.Collections.Generic;

namespace Jeffistance.Services.MessageProcessing
{

    [Flags]
    public enum JeffistanceFlags // Have to be powers of 2 for bitwise operations. Is the bitshifting way better or worse? Leave your comments down below!
    {
        Greeting = 1 << 0,
        Broadcast = 1 << 1,
        Update = 1 << 2,
        Chat = 1 << 3
    }

    class JeffistanceMessageProcessor : MessageProcessor<JeffistanceFlags>
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

        [MessageMethod(JeffistanceFlags.Greeting)]
        private void GreetingFlagMethod(Message message)
        {
            User user = (User) message["User"];
            ClientConnection connection = (ClientConnection) message.Sender;
            user.Connection = connection;
            AppState.GetAppState().Server?.AddUser(user);
        }

        [MessageMethod(JeffistanceFlags.Broadcast)]
        private void BroadcastFlagMethod(Message message)
        {
            message.SetFlags((JeffistanceFlags) message.Flag & ~JeffistanceFlags.Broadcast);
            AppState.GetAppState().MessageHandler.Broadcast(message);
        }

        [MessageMethod(JeffistanceFlags.Chat)]
        private void ChatFlagMethod(Message message)
        {
            message.SetFlags(((JeffistanceFlags) message.Flag | JeffistanceFlags.Update) & ~JeffistanceFlags.Chat);
        }

        [MessageMethod(JeffistanceFlags.Update)]
        private void UpdateFlagMethod(Message message)
        {
            AppState appState = AppState.GetAppState();
            while(message.TryPop(out object result))
            {
                (object obj, string name) = (ValueTuple<object, string>) result;
                PropertyInfo pi = appState.GetType().GetProperty(name);
                pi?.SetValue(appState, obj);
            }
            string username = null;
            if(message.PackedObjects.Count!=0){
                username = (string) message.PackedObjects["username"];
            }
            appState.Log(message.Text, username);
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

        public MessageHandler()
        {
            Processor = new JeffistanceMessageProcessor();
        }

        public void Broadcast(Message message)
        {
           AppState.GetAppState().Server?.Connection.Broadcast(message);
        }

        public void Broadcast(string message)
        {
            Broadcast(new Message(message));
        }

        public void Send(Message message)
        {
            ClientConnection connection = AppState.GetAppState().CurrentUser.Connection;
            connection.Send(message);
        }

        public void Send(string message)
        {
            Send(new Message(message, JeffistanceFlags.Broadcast));
        }

        public void OnMessageReceived(object sender, MessageReceivedArgs args)
        {
            var message = (Message) args.Message;
            Processor.ProcessMessage(message);
        }
    }
}   