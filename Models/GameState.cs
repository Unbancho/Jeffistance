using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Jeffistance.ViewModels;

namespace Jeffistance.Models
{
    public class GameState: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private static GameState _currentGameState;

        public User CurrentUser { get; set; }

        private List<User> _userList;
        public List<User> UserList
        {
            get{ return _userList;}
            set{ _userList = value; OnPropertyChanged();}
        }

        public string Log { get { return ((IChatView)CurrentUser.CurrentWindow).ChatView.Log; } set {((IChatView)CurrentUser.CurrentWindow).ChatView.WriteLineInLog(value+"\n"); } }

        private GameState()
        {

        }

        public static GameState GetGameState()
        {
            return _currentGameState ??= new GameState();
        }

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
