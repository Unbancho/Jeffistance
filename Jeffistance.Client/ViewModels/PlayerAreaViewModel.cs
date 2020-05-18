using ReactiveUI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Jeffistance.Common.Models;
using Jeffistance.Client.Views;
using Jeffistance.Client.Models;

namespace Jeffistance.Client.ViewModels
{
    public class PlayerAreaViewModel : ViewModelBase
    {
        private ObservableCollection <PlayerAreaViewModel> players;

        public ObservableCollection <PlayerAreaViewModel> Players
        {
            get => players;
            set => this.RaiseAndSetIfChanged(ref players, value);
        }

        private CircularPanel circularPanel;

        public CircularPanel CircularPanel
        {
            get => circularPanel;
            set => this.RaiseAndSetIfChanged(ref circularPanel, value);
        }

        public PlayerAreaViewModel()
        {
            CircularPanel = new CircularPanel();
            Players = new ObservableCollection <PlayerAreaViewModel>();
            AppState appState = AppState.GetAppState();
            List<User> userList = appState.UserList;
            foreach(User u in userList)
            {
                PlayerAvatarView pav = new PlayerAvatarView(u.Name);
                CircularPanel.Children.Add(pav);
            }
        }
    }
}
