using Avalonia.Controls;
using ReactiveUI;
using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using Jeffistance.Client.Models;
using System.Collections.Generic;
using Jeffistance.Common.Models;

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
                PlayerAvatarViewModel pavm = new PlayerAvatarViewModel(u.Name);
                CircularPanel.Children.Add(pavm);
            }
        }
    }
}
