using System;
using System.Linq;
using System.ComponentModel;
using System.Reactive;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Client.Models;
using Jeffistance.Common.Models;
using ReactiveUI;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;

namespace Jeffistance.Client.ViewModels
{
    public class LobbyViewModel : ViewModelBase, IChatView
    {
        public ObservableCollection<User> Users {get;}
        bool showKickButton;
        bool showReadyButton;
        bool showStartButton;

        public bool ShowKickButton
        {
            get => showKickButton;
            set => this.RaiseAndSetIfChanged(ref showKickButton, value);
        }

        public bool ShowReadyButton
        {
            get => showReadyButton;
            set => this.RaiseAndSetIfChanged(ref showReadyButton, value);
        }

        public bool ShowStartButton
        {
            get => showStartButton;
            set => this.RaiseAndSetIfChanged(ref showStartButton, value);
        }
        MainWindowViewModel parent;
        private ChatViewModel _chatView;
        public ChatViewModel ChatView 
        {
            get => _chatView;
            set => this.RaiseAndSetIfChanged(ref _chatView, value);
        }

        public ReactiveCommand<Unit, Unit> ReadyUser { get; set; }
        public ReactiveCommand<Unit, Unit> StartGame { get; set ;}

        public LobbyViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            AppState gs = AppState.GetAppState();

            ShowKickButton = gs.CurrentUser.Perms.CanKick;
            ShowReadyButton = !gs.CurrentUser.IsHost;
            ShowStartButton = gs.CurrentUser.IsHost;

            gs.UserList = new List<User>();
            Users = new ObservableCollection<User>(gs.UserList);
            gs.PropertyChanged += OnAppStatePropertyChanged;
            this.ChatView = new ChatViewModel();

            ReadyUser = ReactiveCommand.Create(OnReadyClicked);
        }

        private void OnAppStatePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var property = ((AppState) sender).GetType().GetProperty(args.PropertyName).GetValue(sender);
            if(args.PropertyName == "UserList") // TODO: How can we do it not like this, help
            {
                SyncUserList((List<User>) property);
            }
        }

        private void SyncUserList(List<User> updatedList)
        {
            foreach (var item in updatedList.Except(Users))
            { 
                Dispatcher.UIThread.Post(()=> Users.Add(item));
            }
            foreach (var item in Users.Except(updatedList))
            {
                Dispatcher.UIThread.Post(()=> Users.Remove(item));
            }
        }

        private void OnReadyClicked()
        {
            var user = AppState.GetAppState().CurrentUser;
            Message message = new Message($"{user.Name} is now ready.", JeffistanceFlags.LobbyReady);
            message["UserID"] = user.ID.ToString();
            user.Send(message);
        }

        public void AddReadyUser(Guid userID)
        {
            Console.WriteLine($"Adding user {userID}");
        }
    }
}
