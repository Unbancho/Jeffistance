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
            AppState gs = AppState.GetAppState();
            ShowKickButton = gs.CurrentUser.Perms.CanKick;
            gs.UserList = new List<User>();
            Users = new ObservableCollection<User>(gs.UserList);
            gs.PropertyChanged += OnAppStatePropertyChanged;
            this.ChatView = new ChatViewModel();
        }

        private void OnAppStatePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var property = ((AppState) sender).GetType().GetProperty(args.PropertyName).GetValue(sender);
            if(args.PropertyName == "UserList") // TODO: How can we do it not like this, help
                foreach (var item in ((List<User>) property).Except(Users))
                { 
                    Dispatcher.UIThread.Post(()=> Users.Add(item));
                }
        }
    }
}
