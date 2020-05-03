using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Jeffistance.ViewModels;
using Jeffistance.Services.MessageProcessing;

namespace Jeffistance.Models
{
    public class AppState: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private static AppState _currentAppState;

        public Server Server {get; set;}
        public LocalUser CurrentUser { get; set; }

        private List<User> _userList;
        public List<User> UserList
        {
            get{ return _userList;}
            set{ _userList = value; OnPropertyChanged();}
        }

        public ViewModelBase CurrentWindow {get; set; }

        public MessageHandler MessageHandler {get; set; }

        

        private AppState()
        {

        }

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