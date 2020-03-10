using System.Linq;
using System;
using System.ComponentModel;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Models;
using ReactiveUI;

namespace Jeffistance.ViewModels
{
    public class LobbyViewModel : ViewModelBase, IChatView
    {
        public ObservableCollection<User> Users {get;}
        bool showKickButton;
        public bool ShowKickButton
        {
            get => showKickButton;
            set => this.RaiseAndSetIfChanged(ref showKickButton, value);
        }
        MainWindowViewModel parent;
        private ChatViewModel _chatView;
        public ChatViewModel ChatView 
        {
            get => _chatView;
            set => this.RaiseAndSetIfChanged(ref _chatView, value);
        }

        public LobbyViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            GameState gs = GameState.GetGameState();
            ShowKickButton = gs.CurrentUser.Perms.CanKick;
            Users = new ObservableCollection<User>(gs.CurrentUser.UserList);
            gs.CurrentUser.PropertyChanged += OnUserPropertyChanged;
            this.ChatView = new ChatViewModel();
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
