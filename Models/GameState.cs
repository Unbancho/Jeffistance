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

        public LocalUser CurrentUser { get; set; }

        private List<User> _userList;
        public List<User> UserList
        {
            get{ return _userList;}
            set{ _userList = value; OnPropertyChanged();}
        }

        public ViewModelBase CurrentWindow {get; set; }

        public string Log { 
            get 
            {
                try
                { 
                    return ((IChatView)CurrentWindow).ChatView.Log;
                }
                catch(System.InvalidCastException)
                {
                    return null;
                }
            } 
            set 
            {
                try
                {
                    ((IChatView)CurrentWindow).ChatView.WriteLineInLog(value);
                }
                catch(System.InvalidCastException){}
            } 
        }

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
