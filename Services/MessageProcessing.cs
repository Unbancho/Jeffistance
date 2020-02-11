using System;
using System.Reflection;
using Jeffistance.Models;
using Jeffistance.Services.Messaging;

namespace Jeffistance.Services.MessageProcessing
{

    [Flags]
    enum JeffistanceFlags // Have to be powers of 2 for bitwise operations.
    {
        Greeting = 1,
        Broadcast = 2,
        Update = 4
    }

    public class MessageProcessor
    {
        public void ProcessMessage(Message message)
        {
            object[] parametersToPass = new object[]{message};
            string flagName;
            foreach (Enum flag in message.GetFlags())
            {
                flagName = ((JeffistanceFlags) flag).ToString();
                foreach (var method in message.GetFlaggedMethods(this, flagName))
                {
                    method.Invoke(this, parametersToPass);
                }
            }
        }

        [MessageMethod("Greeting")]
        private void GreetingFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            User user = (User)message["User"];
            ClientConnection connection = (ClientConnection) message.Sender;
            user.Connection = connection;
            host.AddUser(user);
        }

        [MessageMethod("Broadcast")]
        private void BroadcastFlagMethod(Message message)
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            if(!message.HasFlag(JeffistanceFlags.Update)) message = new Message(message.Text);
            host.Broadcast(message);
        }

        [MessageMethod("Update")]
        private void UpdateFlagMethod(Message message)
        {
            User currentUser = GameState.GetGameState().CurrentUser;
            object result;
            while(message.TryPop(out result))
            {
                (object obj, string name) = (ValueTuple<object, string>) result;
                PropertyInfo pi = currentUser.GetType().GetProperty(name, BindingFlags.Instance|BindingFlags.Public);
                pi.SetValue(currentUser, obj);
            }
        }
    }
}