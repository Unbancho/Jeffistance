using System.Linq;
using System;
using System.ComponentModel;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Models;

namespace Jeffistance.ViewModels
{
    public class LobbyViewModel : ViewModelBase
    {
        public ObservableCollection<User> Users {get;}
        MainWindowViewModel parent;

        public LobbyViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            GameState gs = GameState.GetGameState();
            Users = new ObservableCollection<User>(gs.CurrentUser.UserList);
            gs.CurrentUser.PropertyChanged += OnUserPropertyChanged;
        }

        private void OnUserPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var property = ((User) sender).GetType().GetProperty(args.PropertyName).GetValue(sender);
            foreach (var item in ((List<User>) property).Except(Users))
            { 
                Dispatcher.UIThread.Post(()=> Users.Add(item));
            }
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
