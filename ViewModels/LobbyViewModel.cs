using System.Linq;
using System;
using System.ComponentModel;
using System.Reactive;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Models;
using Jeffistance.Services.MessageProcessing;
using ModusOperandi.Messaging;
using ReactiveUI;

namespace Jeffistance.ViewModels
{
    public class LobbyViewModel : ViewModelBase, IChatView
    {
        public ObservableCollection<User> Users { get; }
        
        public ObservableCollection<int> ReadyUserIDs { get; }

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
            ReadyUserIDs = new ObservableCollection<int>();
            gs.PropertyChanged += OnAppStatePropertyChanged;
            this.ChatView = new ChatViewModel();

            ReadyUser = ReactiveCommand.Create(OnReadyClicked);
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

        private void OnReadyClicked()
        {
            Message message = new Message("Ready", JeffistanceFlags.LobbyReady, JeffistanceFlags.Broadcast);
            message["ReadyID"] = AppState.GetAppState().CurrentUser.ID;
            AppState.GetAppState().MessageHandler.Send(message);
        }
        
        public void UpdateReadyPlayers(int id)
        {
            Console.WriteLine($"New ready: {id}");
        }
    }
}
