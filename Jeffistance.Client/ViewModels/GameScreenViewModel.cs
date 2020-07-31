using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Threading;
using Jeffistance.Client.Models;
using Jeffistance.Client.Views;
using Jeffistance.Common.AvaloniaTools;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services;
using Jeffistance.Common.Services.IoC;
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
        public int _selectablePlayers;
        private string _roundBox;
        private Dictionary<int, ScoreNodeView> _scoreDictionary;
        private List<string> SelectedUserIDs;
        List<PlayerAvatarView> AvatarsList;
        public GameState GameState;
        public List<string> TeamPickedUsersIDs;
        public string _noChoice;
        public string _yesChoice;
        public GameScreenViewModel()
        {
            GameState = new GameState();
            PlayerArea = new PlayerAreaViewModel();
            ScoreDictionary = new Dictionary<int, ScoreNodeView>();
            ChatView = new ChatViewModel();
            ScorePanel = new StackPanel();
            AvatarsList = new List<PlayerAvatarView>();
            NoChoice = "Gray";
            YesChoice = "Orange";

            EnableOKBtn = true;
            EnableVotingBtns = false;

            SelectablePlayers = 0;
            SelectedUserIDs = new List<string>();

            ///Things that maybe should be moved
            TeamPickedUsersIDs = new List<string>();
            
            //Adding score nodes
            for (int index = 0; index < 5; index++)
            {
                ScoreNodeView snv = new ScoreNodeView();
                ScorePanel.Children.Add(snv);
                ScoreDictionary.Add(index, snv);
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

        public void PrepareAvatars(List<Player> Players)
        {
            foreach (Player p in Players)
            {
                PlayerAvatarView pav = new PlayerAvatarView(p.Name, p.UserID.ToString());
                pav.PointerPressed += onAvatarClicked;
                PlayerArea.CircularPanel.Children.Add(pav);
                AvatarsList.Add(pav);
            }
        }

        private void onAvatarClicked(object sender, PointerPressedEventArgs args)
        {
            if (SelectablePlayers != 0)
            {
                PlayerAvatarView playerAvatar = (PlayerAvatarView)sender;
                if (!SelectedUserIDs.Contains(playerAvatar.UserId))
                {
                    if (SelectedUserIDs.Count < SelectablePlayers)
                    {
                        playerAvatar.Avatar.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Selected Spy.png");
                        SelectedUserIDs.Add(playerAvatar.UserId);
                    }
                }
                else
                {
                    playerAvatar.Avatar.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Spy.png");
                    SelectedUserIDs.Remove(playerAvatar.UserId);
                }
            }
        }

        internal void RandomRoundResult()
        {
            ScoreNodeView scoreNode = ScoreDictionary[GameState.CurrentRound];
            scoreNode.ChangeState(false);
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

        public string NoChoice
        {
            get => _noChoice;
            set => this.RaiseAndSetIfChanged(ref _noChoice, value);
        }
        public string YesChoice
        {
            get => _yesChoice;
            set => this.RaiseAndSetIfChanged(ref _yesChoice, value);
        }

        public Boolean EnableVotingBtns
        {
            get => _enableVotingBtns;
            set => this.RaiseAndSetIfChanged(ref _enableVotingBtns, value);
        }

        internal void DeclareLeader(int teamSize, Player leader)
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeDeclareLeaderMessage(teamSize, leader);
            user.Send(message);
        }

        internal void ShowSelectedPlayers()
        {
            foreach (PlayerAvatarView pav in AvatarsList)
            {
                if (TeamPickedUsersIDs.Contains(pav.UserId))
                {       
                    pav.Avatar.Source  = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Selected Spy.png");
                }
            }
        }

        public void RestorePlayersToNormal()
        {
            foreach (PlayerAvatarView pav in AvatarsList)
            {
                pav.Avatar.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Spy.png");
            }
            SelectedUserIDs = new List<string>();
        }

        internal void ChangeOKBtnState(bool v)
        {
            EnableOKBtn = v;
        }

        public Dictionary<int, ScoreNodeView> ScoreDictionary
        {
            get => _scoreDictionary;
            set => this.RaiseAndSetIfChanged(ref _scoreDictionary, value);
        }
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


        public ReactiveCommand<Unit, Unit> OnOkClicked { get; }
        public ReactiveCommand<Unit, Unit> OnYesClicked { get; }
        public ReactiveCommand<Unit, Unit> OnNoClicked { get; }

        public void OnOkClickedMethod()
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            if (GameState.CurrentPhase == Phase.TeamPicking) //Leader picking team 
            {
                if (SelectedUserIDs.Count == SelectablePlayers)
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

        internal void MakeShowVotingResultMessage(Dictionary<string, bool> voters, bool successfulTeamFormation, int fails)
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeShowTeamVoteResultMessage(voters, successfulTeamFormation, fails);
            user.Send(message);
        }

        internal void StartMissionVoting()
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            var message = messageFactory.MakeStartMissionVotingMessage(TeamPickedUsersIDs);
            user.Send(message);
        }

        public void OnYesClickedMethod()
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            NoChoice = "Gray";
            YesChoice = "Orange";
            switch(GameState.CurrentPhase)
            {
                case Phase.TeamVoting:
                    var message = messageFactory.MakeVoteMessage(user.ID.ToString(), true);
                    user.Send(message);
                    break;
                case Phase.MissionVoting:
                    var missionMessage = messageFactory.MakeMissionVoteMessage(user.ID.ToString(), true);
                    user.Send(missionMessage);
                    break;
            }

        }

        public void OnNoClickedMethod()
        {
            AppState gs = AppState.GetAppState();
            var user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            NoChoice = "Orange";
            YesChoice = "Gray";
            switch(GameState.CurrentPhase)
            {
                case Phase.TeamVoting:
                    var message = messageFactory.MakeVoteMessage(user.ID.ToString(), false);
                    user.Send(message);
                    break;
                case Phase.MissionVoting:
                    var missionMessage = messageFactory.MakeMissionVoteMessage(user.ID.ToString(), false);
                user.Send(missionMessage);
                    break;
            }
        }

        internal void ShowMissionVotingInterface()
        {
            EnableVotingBtns = true;
        }

        internal void ShowTeamVoteResult(Dictionary<string, bool> voters, bool result, int fails)
        {
            AppState appState = AppState.GetAppState();
            EnableVotingBtns = false;
            EnableOKBtn = true;
            RoundBox = "";
            if (result)
            {
                GameState.CurrentPhase = Phase.ShowingTeamVoteResult;
            }
            else
            {
                RestorePlayersToNormal();
                if (fails == 0) //if the maximum of fails was reached
                {
                    GameState.CurrentPhase = Phase.AssigningRandomResult;
                }
                else
                {
                    GameState.CurrentPhase = Phase.FailedTeamFormation;
                    RoundBox += "Team failed to form. " + fails + "/5 ";
                }

            }
            foreach (string u in voters.Keys)
            {
                List<User> userList = appState.UserList;
                string playerName = userList.Find(user => user.ID.ToString() == u).Name;
                RoundBox += "[" + playerName + ": "; //TODO Make this prettier with a proper label
                if (voters[u])
                {
                    RoundBox += "Yes] ";
                }
                else
                {
                    RoundBox += "No] ";
                }
            }
        }

        internal void ResolveMissionResult(bool missionSucceeds)
        {
            ScoreNodeView scoreNode = ScoreDictionary[GameState.CurrentRound];
            scoreNode.ChangeState(missionSucceeds);
            GameState.CurrentRound++;
            if (missionSucceeds)
            {
                RoundBox = "Mission successful";
            }
            else
            {
                RoundBox = "Mission failed";
            }
            EnableOKBtn = true;
            EnableVotingBtns = false;
            GameState.CurrentPhase = Phase.Standby;
        }

        internal void ShowEndGameResults(string winningFactionName, List<string> spiesIDs)
        {
            EnableOKBtn = false;
            EnableVotingBtns = false;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            foreach (PlayerAvatarView avatar in AvatarsList)
            {
                if (spiesIDs.Contains(avatar.UserId))
                {
                    avatar.Avatar.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Jew.png");
                }
                else
                {
                    avatar.Avatar.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Spy.png"); //changes them back in case they were selected
                }
            }
            RoundBox = winningFactionName + " victory!";
        }

    }

}