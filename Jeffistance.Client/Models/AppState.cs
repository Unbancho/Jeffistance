using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Jeffistance.Client.ViewModels;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using Jeffistance.JeffServer.Models;

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

        public string Log { 
            get {return (CurrentWindow as IChatView)?.ChatView.Log;} 
            set {(CurrentWindow as IChatView)?.ChatView.WriteLineInLog(value, null);} 
        }

        private AppState(){}

        public static AppState GetAppState()
        {
            return _currentAppState ??= new AppState();
        }

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Log(string text, string name)
        {
            if(name==null){
                name = AppState.GetAppState().CurrentUser.Name;
            }
            ((IChatView)CurrentWindow)?.ChatView.WriteLineInLog(text, name);
        }
        
    }
}