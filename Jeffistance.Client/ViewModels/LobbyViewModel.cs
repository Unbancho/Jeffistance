using System;
using System.Linq;
using System.ComponentModel;
using System.Reactive;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Client.Models;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services.IoC;
using Jeffistance.Common.Services;
using ReactiveUI;
using Jeffistance.JeffServer.Models;

namespace Jeffistance.Client.ViewModels
{
    public class LobbyViewModel : ViewModelBase, IChatView
    {
        public ObservableCollection<User> Users { get; }
        public List<Guid> ReadyUserIDs { get; }
        private bool showKickButton;
        private bool showReadyButton;
        private bool showStartButton;
        private bool canStart;

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

        public bool CanStart
        {
            get => canStart;
            set => this.RaiseAndSetIfChanged(ref canStart, value);
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
            ReadyUserIDs = new List<Guid>();

            gs.PropertyChanged += OnAppStatePropertyChanged;
            this.ChatView = new ChatViewModel();

            ReadyUser = ReactiveCommand.Create(OnReadyClicked);

            var startEnabled = this.WhenAnyValue(
                x => x.CanStart
            );
            StartGame = ReactiveCommand.Create(OnStartClicked, startEnabled);
        }

        private void OnAppStatePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var property = ((AppState) sender).GetType().GetProperty(args.PropertyName).GetValue(sender);
            if(args.PropertyName == "UserList") // TODO: How can we do it not like this, help
            {
                Dispatcher.UIThread.Post(() => SyncUserList((List<User>) property));
            }
        }

        private void SyncUserList(List<User> updatedList)
        {
            foreach (var item in updatedList.Except(Users))
            { 
                Users.Add(item);
            }

            foreach (var item in Users.ToList().Except(updatedList))
            {
                Users.Remove(item);
            }
        }

        private void OnReadyClicked()
        {
            var user = AppState.GetAppState().CurrentUser;

            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            // Send ready message
            var message = messageFactory.MakeLobbyReadyMessage(user.ID);
            user.Send(message);
        }

        public void AddReadyUser(Guid userID)
        {
            if (ReadyUserIDs.Contains(userID))
            {
                ReadyUserIDs.Remove(userID);
            }
            else
            {
                ReadyUserIDs.Add(userID);
            }
        }

        public void OnEveryoneReadyStateChange(bool ready)
        {
            Dispatcher.UIThread.Post(() => CanStart = ready);
        }

        private void OnStartClicked()
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();

            var message = messageFactory.MakeJoinGameMessage();
            user.Send(message);
        }

        public void MoveToGameScreen()
        {
            AppState gs = AppState.GetAppState();
            GameScreenViewModel gameScreen = new GameScreenViewModel();
            gs.CurrentWindow = gameScreen;
            parent.Content = gameScreen;
        }
    }
}
