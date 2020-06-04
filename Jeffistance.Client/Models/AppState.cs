using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Jeffistance.Client.ViewModels;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Models;
using System.Linq;

namespace Jeffistance.Client.Models
{
    public class AppState: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private static AppState _currentAppState;

        public string[] Arguments {get; set;}

        public Server Server {get; set;}
        public LocalUser CurrentUser { get; set; }

        private List<User> _userList;
        public List<User> UserList
        {
            get{ return _userList;}
            set
            { 
                _userList = value; 
                OnPropertyChanged();
            }
        }

        public ViewModelBase CurrentWindow { get; set; }

        public LobbyViewModel CurrentLobby { get; set; }

        public MessageHandler MessageHandler {get; set; }

        private AppState(){}

        public static AppState GetAppState()
        {
            return _currentAppState ??= new AppState();
        }

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Log(string text, string name, string msgId)
        {
            /*
            if(name==null){
                name = "Server Announcer";
            }
            */
            //If no msg id is passed through the Message, then a local one is generated
            string id = Guid.NewGuid().ToString();
            if(msgId != null){
                id = msgId;
            }
            (CurrentWindow as IChatView)?.ChatView.WriteLineInLog(text, name, id);
        }
        internal void DeleteChatMessage(string msgId)
        {
            (CurrentWindow as IChatView)?.ChatView.DeleteMessage(msgId);
        }
        internal void EditChatMessage(string msgId, string newText)
        {
            (CurrentWindow as IChatView)?.ChatView.EditMessage(msgId, newText);
        }

        public User GetUserByID(Guid userID)
        {
            return UserList.FirstOrDefault((user) => user.ID == userID);
        }

        public User GetUserByID(string userID)
        {
            return GetUserByID(Guid.Parse(userID));
        }
        
    }
}