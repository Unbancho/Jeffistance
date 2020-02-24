using System.Collections.ObjectModel;
using Jeffistance.Models;

using System.Linq;
using System;

namespace Jeffistance.ViewModels
{
    public class LobbyViewModel : ViewModelBase
    {
        public ObservableCollection<User> Users { get; }
        MainWindowViewModel parent;

        public LobbyViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            GameState gs = GameState.GetGameState();
            Users = gs.CurrentUser.UserList;
        }

        public void OnKickEveryoneClicked()
        {
            Host host = (Host) GameState.GetGameState().CurrentUser;
            foreach(User u in host.UserList.ToList())
            {
                host.Kick(u);
            }
            Console.WriteLine(host.UserList.Count);
        }
    }
}
