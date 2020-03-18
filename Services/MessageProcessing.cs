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

    public class JeffistanceMessageProcessor : MessageProcessor<JeffistanceFlags>
    {
        public override Dictionary<JeffistanceFlags, MethodInfo> ProcessingMethods {get; set;}
        public JeffistanceMessageProcessor()
        {
            ProcessingMethods = new Dictionary<JeffistanceFlags, MethodInfo>();
            foreach (var processingMethod in GetType().GetMethods(BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(y => y.GetCustomAttributes().OfType<MessageMethodAttribute>().Any()))
            {
                ProcessingMethods[((MessageMethodAttribute) processingMethod.GetCustomAttribute(typeof(MessageMethodAttribute))).Flag] = processingMethod;
            }
        }
    }

    public class HostMessageProcessor : JeffistanceMessageProcessor
    {
        [MessageMethod(JeffistanceFlags.Greeting)]
        private void GreetingFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            User user = (User) message["User"];
            ClientConnection connection = (ClientConnection) message.Sender;
            user.Connection = connection;
            host.AddUser(user);
        }

        [MessageMethod(JeffistanceFlags.Broadcast)]
        private void BroadcastFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            message.SetFlags((JeffistanceFlags) message.Flag | ~JeffistanceFlags.Broadcast);
            host.Broadcast(message);
        }

        [MessageMethod(JeffistanceFlags.Chat)]
        private void ChatFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            message.SetFlags((JeffistanceFlags) message.Flag | JeffistanceFlags.Update & ~JeffistanceFlags.Chat);
            message["Log"] = message.Text;
        }
    }

    public class UserMessageProcessor : JeffistanceMessageProcessor
    {
        [MessageMethod(JeffistanceFlags.Update)]
        private void UpdateFlagMethod(Message message)
        {
            GameState gameState = GameState.GetGameState();
            while(message.TryPop(out object result))
            {
                (object obj, string name) = (ValueTuple<object, string>) result;
                PropertyInfo pi = gameState.GetType().GetProperty(name);
                pi?.SetValue(gameState, obj);
            }
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
}   