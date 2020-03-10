using System;
using System.Reflection;
using Jeffistance.Models;
using Jeffistance.Services.Messaging;
using System.Linq;
using System.Collections.Generic;

namespace Jeffistance.Services.MessageProcessing
{

    [Flags]
    enum JeffistanceFlags // Have to be powers of 2 for bitwise operations. Is the bitshifting way better or worse? Leave your comments down below!
    {
        Greeting = 1 << 0,
        Broadcast = 1 << 1,
        Update = 1 << 2,
        Chat = 1 << 3
    }

    public abstract class JeffistanceMessageProcessor : MessageProcessor
    {
        public JeffistanceMessageProcessor()
        {
            FlagType = typeof(JeffistanceFlags);
            ProcessingMethods = new Dictionary<Enum, MethodInfo>();
            foreach (var processingMethod in GetType().GetMethods(BindingFlags.NonPublic|BindingFlags.Instance)
                .Where(y => y.GetCustomAttributes().OfType<MessageMethodAttribute>().Any()))
            {
                ProcessingMethods[(JeffistanceFlags) Enum.Parse(typeof(JeffistanceFlags), ((MessageMethodAttribute) processingMethod.GetCustomAttribute(typeof(MessageMethodAttribute))).FlagName)] = processingMethod;
            }
        }
    }

    public class HostMessageProcessor : JeffistanceMessageProcessor
    {
        [MessageMethod("Greeting")]
        private void GreetingFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            User user = (User) message["User"];
            ClientConnection connection = (ClientConnection) message.Sender;
            user.Connection = connection;
            host.AddUser(user);
        }

        [MessageMethod("Broadcast")]
        private void BroadcastFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            message.SetFlags((JeffistanceFlags) message.Flag | ~JeffistanceFlags.Broadcast);
            host.Broadcast(message);
        }

        [MessageMethod("Chat")]
        private void ChatFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            message.SetFlags((JeffistanceFlags) message.Flag | JeffistanceFlags.Update & ~JeffistanceFlags.Chat);
            message["Log"] = message.Text;
        }
    }

    public class UserMessageProcessor : JeffistanceMessageProcessor
    {
        [MessageMethod("Update")]
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
        public string FlagName;

        public MessageMethodAttribute(string flagName)
        {
            FlagName = flagName;
        }
    }
}   