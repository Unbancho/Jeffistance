using System.Collections.ObjectModel;
using Jeffistance.Models;

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
            Users = ((Host)gs.CurrentUser).UserList;
        }
    }
}
