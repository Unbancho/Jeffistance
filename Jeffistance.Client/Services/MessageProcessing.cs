using System;
using System.Reflection;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Client.Models;
using ModusOperandi.Messaging;
using Jeffistance.Common.Models;

namespace Jeffistance.Client.Services.MessageProcessing
{
    class ClientMessageProcessor : JeffistanceMessageProcessor
    {
        public override void ProcessMessage(Message message)
        {
            base.ProcessMessage(message);
            // Why Bancho
            // LogMessage(message);
        }

        public override void LogMessage(Message message)
        {
            var appState = AppState.GetAppState();
            appState.Log(message.Text, (string) message.Sender);
        }

        [MessageMethod(JeffistanceFlags.Update)]
        private void UpdateFlagMethod(Message message)
        {
            var appState = AppState.GetAppState();
            while(message.TryPop(out object result))
            {
                (object obj, string name) = (ValueTuple<object, string>) result;
                PropertyInfo pi = appState.GetType().GetProperty(name);
                pi?.SetValue(appState, obj);
            }

        }

        [MessageMethod(JeffistanceFlags.LobbyReady)]
        private void LobbyReadyFlagMethod(Message message)
        {
            AppState.GetAppState().CurrentLobby.AddReadyUser(Guid.Parse((string) message["UserID"]));
        }

        [MessageMethod(JeffistanceFlags.Chat)]
        private void ChatFlagMessage(Message message)
        {
            // do chat things
            AppState appState = AppState.GetAppState();
            User user = null;
            
            if(message.TryPop(out object userId, "UserID"))
            {
                user = appState.GetUserByID((string) userId);
                appState.Log(message.Text, user.Name);
            }
            else
            {   
                appState.Log(message.Text, "");
            }
        }
    }
}
