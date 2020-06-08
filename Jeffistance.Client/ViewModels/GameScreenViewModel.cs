using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Jeffistance.Client.Models;
using Jeffistance.Client.Views;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services;
using Jeffistance.Common.Services.IoC;
using Jeffistance.JeffServer.Models;
using ReactiveUI;

namespace Jeffistance.Client.ViewModels
{
    public class GameScreenViewModel : ViewModelBase, IChatView
    {
        private PlayerAreaViewModel _playerArea;
        private ChatViewModel _chatView;
        private StackPanel _scorePanel;
        public Boolean _enableOKBtn;
        public Boolean _enableVotingBtns;
        private Server _server;
        public int _selectablePlayers;
        private string _roundBox;
        //private Dictionary<int, ScoreNodeView> _scoreDictionary;
        private List<Guid> ReadyUserIDs;
        private List<string> SelectedUserIDs;
        public string CurrentLeaderID;
        public Phase CurrentPhase;
        List<PlayerAvatarView> AvatarsList;
        public GameState GameState;
        public List<string> TeamPickedUsersIDs;
        public GameScreenViewModel()
        {
            PlayerArea = new PlayerAreaViewModel();
            //ScoreDictionary = new Dictionary<int, ScoreNodeView>();
            ChatView = new ChatViewModel();
            ScorePanel = new StackPanel();
            TeamPickedUsersIDs = new List<string>();
            EnableOKBtn = true;
            EnableVotingBtns = false;
            SelectablePlayers = 0;
            AppState ass = AppState.GetAppState();
            Server = ass.Server;
            ReadyUserIDs = new List<Guid>();
            SelectedUserIDs = new List<string>();
            AvatarsList = new List<PlayerAvatarView>();
            List<User> Users = ass.UserList;
            GameState = new GameState(); //TODO actually load stuff in here instead of making a new poperty

            //Adding players
            //List<User> userList = AppState.UserList;
            foreach(User u in Users)
            {
                PlayerAvatarView pav = new PlayerAvatarView(u.Name, u.ID.ToString());
                pav.PointerPressed += onAvatarClicked;
                PlayerArea.CircularPanel.Children.Add(pav);
                AvatarsList.Add(pav);
            }
            //Adding score nodes
            for (int index = 1; index <= 5; index++)
            {               
                ScoreNodeView snv = new ScoreNodeView();
                ScorePanel.Children.Add(snv);
                //ScoreDictionary.Add(index, snv);
            }

            var okEnabled = this.WhenAnyValue(
                x => x.EnableOKBtn,
                x => x == true);

            var votingEnabled = this.WhenAnyValue(
                x => x.EnableVotingBtns,
                x => x == true);

            OnOkClicked = ReactiveCommand.Create(OnOkClickedMethod, okEnabled);
            OnYesClicked = ReactiveCommand.Create(OnYesClickedMethod, votingEnabled);
            OnNoClicked = ReactiveCommand.Create(OnNoClickedMethod, votingEnabled);
        }

        public void AddReadyUser(Guid userID)
        {
            if (!ReadyUserIDs.Contains(userID))
            {
                ReadyUserIDs.Add(userID);
            }
            if (AppState.GetAppState().CurrentUser.IsHost)
            {
                Dispatcher.UIThread.Post(CheckIfAllReady);
            }
        }

        private void CheckIfAllReady()
        {
            AppState ass = AppState.GetAppState();
            List<User> Users = ass.UserList;
            if (ass.CurrentUser.IsHost && Users.Count == ReadyUserIDs.Count) //Works as long as theres no expectators, which there arent
            {
                AppState gs = AppState.GetAppState();
                var user = AppState.GetAppState().CurrentUser;
                var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
                var message = messageFactory.MakeAdvanceGamePhaseMessage();
                user.Send(message);
            }
        }

        private void onAvatarClicked(object sender, PointerPressedEventArgs args)
        {
            if(SelectablePlayers != 0)
            {
                PlayerAvatarView playerAvatar = (PlayerAvatarView) sender;
                if(!SelectedUserIDs.Contains(playerAvatar.UserId))
                {
                    if(SelectedUserIDs.Count < SelectablePlayers)
                    {
                        playerAvatar.Avatar.Source =  new Bitmap("Jeffistance.Client\\Assets\\Vorebisu.png");
                        SelectedUserIDs.Add(playerAvatar.UserId);   
                    }
                }
                else
                {
                    SelectedUserIDs.Remove(playerAvatar.UserId); 
                    playerAvatar.Avatar.Source =  new Bitmap("Jeffistance.Client\\Assets\\Spy.png");
                    
                }
            }
        }
        
        public StackPanel ScorePanel
        {
            get => _scorePanel;
            set => this.RaiseAndSetIfChanged(ref _scorePanel, value);
        }

        internal void ChangeVotingBtnsState(bool v)
        {
           EnableVotingBtns = v;
        }

        public Boolean EnableOKBtn
        {
            get => _enableOKBtn;
            set => this.RaiseAndSetIfChanged(ref _enableOKBtn, value);
        }

        public Boolean EnableVotingBtns
        {
            get => _enableVotingBtns;
            set => this.RaiseAndSetIfChanged(ref _enableVotingBtns, value);
        }

        internal void DeclareLeader(int teamSize, string leaderID)
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeDeclareLeaderMessage(teamSize, leaderID);
            user.Send(message);
        }

        internal void ShowSelectedPlayers()
        {
            foreach(PlayerAvatarView pav in AvatarsList)
            {
                if(TeamPickedUsersIDs.Contains(pav.UserId))
                {
                    pav.Avatar.Source =  new Bitmap("Jeffistance.Client\\Assets\\Vorebisu.png");
                }
            }
        }

        internal void ChangeOKBtnState(bool v)
        {
            EnableOKBtn = v;
        }

        /*
        public Dictionary<int, ScoreNodeView> ScoreDictionary
        {
            get => _scoreDictionary;
            set => this.RaiseAndSetIfChanged(ref _scoreDictionary, value);
        }   
        */
        public PlayerAreaViewModel PlayerArea
        {
            get => _playerArea;
            set => this.RaiseAndSetIfChanged(ref _playerArea, value);
        }

        public int SelectablePlayers
        {
            get => _selectablePlayers;
            set => this.RaiseAndSetIfChanged(ref _selectablePlayers, value);
        }

        public ChatViewModel ChatView
        {
            get => _chatView;
            set => this.RaiseAndSetIfChanged(ref _chatView, value);
        }
        public string RoundBox
        {
            get => _roundBox;
            set => this.RaiseAndSetIfChanged(ref _roundBox, value);
        }

        public Server Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }
        
        public ReactiveCommand<Unit, Unit> OnOkClicked { get; }
        public ReactiveCommand<Unit, Unit> OnYesClicked { get; }
        public ReactiveCommand<Unit, Unit> OnNoClicked { get; }

        public void OnOkClickedMethod(){
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            if(CurrentPhase == Phase.TeamPicking) //Leader picking team 
            {
                if(SelectedUserIDs.Count == SelectablePlayers)
                {
                    var message = messageFactory.MakePickTeamMessageMessage(SelectedUserIDs);
                    user.Send(message);
                }
                else
                {
                    return; //Not enough players selected to form the team
                }
            }
            else //OK information
            {
                var message = messageFactory.MakeGamePhaseReadyMessage(user.ID.ToString());
                user.Send(message);
            }
            EnableOKBtn = false;
        }

        internal void StartMissionVoting(Dictionary<string, bool> voters)
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeStartMissionVotingMessage(voters);
            user.Send(message);
        }

        public void OnYesClickedMethod(){
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeVoteMessage(user.ID.ToString(), true);
            user.Send(message);
        }

        public void OnNoClickedMethod(){
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeVoteMessage(user.ID.ToString(), false);
            user.Send(message);
        }
    }

}