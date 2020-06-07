using System;
using System.Reflection;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Client.Models;
using ModusOperandi.Messaging;
using Jeffistance.Client.ViewModels;
using Avalonia.Threading;
using System.Collections.Generic;
using Jeffistance.Common.Models;

namespace Jeffistance.Client.Services.MessageProcessing
{
    class ClientMessageProcessor : JeffistanceMessageProcessor
    {
        public override void ProcessMessage(Message message)
        {
            base.ProcessMessage(message);
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
            AppState appState = AppState.GetAppState();
            
            string username = message.TryPop(out object userId, "UserID")?
            appState.GetUserByID((string) userId).Name : null;
            string MessageID = message.TryPop(out object msgId, "MessageID")?
            (string)msgId: null;
            appState.Log(message.Text, username, MessageID);
        }

        [MessageMethod(JeffistanceFlags.EditChatMessage)]
        private void EditChatMessageFlagMethod(Message message)
        {
            AppState appState = AppState.GetAppState();
            string msgId = (string) message["MessageID"];
            string newText = (string) message["NewText"];
            appState.EditChatMessage(msgId, newText);
        }
        
        [MessageMethod(JeffistanceFlags.DeleteChatMessage)]
        private void DeleteChatMessageFlagMethod(Message message)
        {
            AppState appState = AppState.GetAppState();
            string msgId = (string) message["MessageID"];
            appState.DeleteChatMessage(msgId);
        }

        [MessageMethod(JeffistanceFlags.JoinGameMessage)]
        private void JoinGameMessageFlagMethod(Message message)
        {
            LobbyViewModel lobby = AppState.GetAppState().CurrentLobby;
            Dispatcher.UIThread.Post(()=> lobby.MoveToGameScreen());
            Dispatcher.UIThread.Post(()=> lobby.SetupGame());
        }

        [MessageMethod(JeffistanceFlags.GetPlayerInfoMessage)]
        private void GetPlayerInfoMessageFlagMethod(Message message)
        {
            AppState appState = AppState.GetAppState();
            List<Player> players = (List<Player>) message["Players"];
            Player me = players.Find(x => x.UserID == appState.CurrentUser.ID.ToString());
            (appState.CurrentWindow as GameScreenViewModel).RoundBox = "You are in the " + me.Faction.Name + " team";
        }

        [MessageMethod(JeffistanceFlags.GamePhaseReadyMessage)]
        private void GamePhaseReadyMessageFlagMethod(Message message)
        {
            (AppState.GetAppState().CurrentWindow as GameScreenViewModel)
            .AddReadyUser(Guid.Parse((string) message["UserID"]));
        }
        
        [MessageMethod(JeffistanceFlags.AdvanceGamePhaseMessage)]
        private void AdvanceGamePhaseMessageFlagMethod(Message message)
        {
            if(AppState.GetAppState().CurrentUser.IsHost)
            {
                Game game = AppState.GetAppState().Server.Game;
                int teamSize = game.NextTeamSize;
                string leaderID = game.CurrentLeader.UserID;
                (AppState.GetAppState().CurrentWindow as GameScreenViewModel).DeclareLeader(teamSize, leaderID);
            }
        }

        [MessageMethod(JeffistanceFlags.PickTeamMessage)]
        private void PickTeamMessageFlagMethod(Message message)
        {
            List<string> pickedUsers = (List<string>) message["PlayersInTeamIDs"];
            AppState appState = AppState.GetAppState();
            GameScreenViewModel gameScreen = (appState.CurrentWindow as GameScreenViewModel);
            string leaderName = appState.UserList.Find(u => u.ID.ToString() == gameScreen.CurrentLeaderID).Name;
            Dispatcher.UIThread.Post(()=>gameScreen.ShowSelectedPlayers(pickedUsers));
            gameScreen.RoundBox = leaderName + " picked the following team. It's voting time";
            gameScreen.CurrentPhase = Phase.TeamVoting;
            Dispatcher.UIThread.Post(()=> gameScreen.ChangeOKBtnState(true));
            User me = appState.CurrentUser;
            if(me.ID.ToString().Equals(gameScreen.CurrentLeaderID))
            {
                gameScreen.SelectablePlayers = 0;
            }
        }
        
        [MessageMethod(JeffistanceFlags.DeclareLeaderMessage)]
        private void DeclareLeaderMessageFlagMethod(Message message)
        {
            string leaderID =  (string) message["UserID"];
            int teamSize =  (int) message["TeamSize"];
            AppState appState = AppState.GetAppState();
            GameScreenViewModel gameScreen = (appState.CurrentWindow as GameScreenViewModel);
            gameScreen.CurrentLeaderID = leaderID;
            User me = appState.CurrentUser;
            if(me.ID.ToString().Equals(leaderID))
            {
                gameScreen.SelectablePlayers = teamSize;
                Dispatcher.UIThread.Post(()=> gameScreen.ChangeOKBtnState(true));
                gameScreen.RoundBox = "Pick a team of " + teamSize + " players for the next mission";
                gameScreen.CurrentPhase = Phase.TeamPicking;
                
            }
            else
            {
                string leaderName = appState.UserList.Find(u => u.ID.ToString()==leaderID).Name;
                Dispatcher.UIThread.Post(()=> gameScreen.ChangeOKBtnState(false));
                gameScreen.RoundBox = 
                leaderName + " is picking a team of " + teamSize + " for the next mission";
            }
        }
        
        
    }
}
