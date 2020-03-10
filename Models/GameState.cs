using System.Collections.Generic;
using Jeffistance.ViewModels;

namespace Jeffistance.Models
{
    public class GameState
    {
        private static GameState _currentGameState;

        public User CurrentUser { get; set; }

        public List<User> UserList { get { return CurrentUser.UserList; } set {CurrentUser.UserList = value; } }

        public string Log { get { return ((IChatView)CurrentUser.CurrentWindow).ChatView.Log; } set {((IChatView)CurrentUser.CurrentWindow).ChatView.WriteLineInLog(value+"\n"); } }

        private GameState()
        {

        }

        public static GameState GetGameState()
        {
            return _currentGameState ??= new GameState();
        }
    }
}
