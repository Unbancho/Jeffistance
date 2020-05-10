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
            Guid id = Guid.NewGuid();
            if(msgId != null){
                id = Guid.Parse(msgId);
            }
            ((IChatView)CurrentWindow)?.ChatView.WriteLineInLog(text, name, id);
        }
        internal void DeleteChatMessage(string msgId)
        {
            ((IChatView)CurrentWindow)?.ChatView.DeleteMessage(msgId);
        }
        internal void EditChatMessage(string msgId, string newText)
        {
            ((IChatView)CurrentWindow)?.ChatView.EditMessage(msgId, newText);
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