using System;
using System.Collections.Generic;
using System.Reactive;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Jeffistance.Client.Models;
using Jeffistance.Client.Views;
using Jeffistance.Common.AvaloniaTools;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services;
using Jeffistance.Common.Services.IoC;
using ReactiveUI;
using ModusOperandi.Messaging;

namespace Jeffistance.Client.ViewModels
{
    public class GameScreenViewModel : ViewModelBase, IChatView
    {
        private AppState _as;
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
        public GameState GameState { get; set; }
        public List<string> TeamPickedUsersIDs;
        public string _noChoice;
        public string _yesChoice;
        private Player _currentPlayer;
        public Player CurrentPlayer
        {
            get
            {
                return _currentPlayer ??= FindCurrentPlayer();
            }
        }

        public GameScreenViewModel()
        {
            _as = AppState.GetAppState();
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

        public void PrepareAvatars()
        {
            foreach (Player p in GameState.Players)
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
            var user = _as.CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            Message message;
            switch (GameState.CurrentPhase)
            {
                case Phase.TeamPicking: //Leader picking team 
                    if (SelectedUserIDs.Count == SelectablePlayers)
                    {
                        message = messageFactory.MakePickTeamMessage(SelectedUserIDs);
                        user.Send(message);
                    }
                    else
                    {
                        return; //Not enough players selected to form the team
                    }
                    break;
                
                case Phase.MissionVoting:
                    if (GameState.CurrentTeam.Contains(CurrentPlayer))
                    {
                        RoundBox = "Vote on the missions's success!";
                        ShowMissionVotingInterface();
                    }
                    else
                    {
                        RoundBox = "The current team is voting on the mission's success.";
                    }
                    break;

                default: //OK information
                    message = messageFactory.MakeGamePhaseReadyMessage(user.ID.ToString());
                    user.Send(message);
                    break;
            }
            EnableOKBtn = false;
        }

        public void OnYesClickedMethod()
        {
            var user = _as.CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            NoChoice = "Gray";
            YesChoice = "Orange";
            switch(GameState.CurrentPhase)
            {
                case Phase.TeamVoting:
                    var message = messageFactory.MakeVoteMessage(CurrentPlayer.ID, true);
                    user.Send(message);
                    break;
                case Phase.MissionVoting:
                    var missionMessage = messageFactory.MakeMissionVoteMessage(CurrentPlayer.ID, true);
                    user.Send(missionMessage);
                    break;
            }
            EnableVotingBtns = false;
        }

        public void OnNoClickedMethod()
        {
            var user = _as.CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();
            NoChoice = "Orange";
            YesChoice = "Gray";
            switch(GameState.CurrentPhase)
            {
                case Phase.TeamVoting:
                    var message = messageFactory.MakeVoteMessage(CurrentPlayer.ID, false);
                    user.Send(message);
                    break;
                case Phase.MissionVoting:
                    var missionMessage = messageFactory.MakeMissionVoteMessage(CurrentPlayer.ID, false);
                    user.Send(missionMessage);
                    break;
            }
            EnableVotingBtns = false;
        }

        internal void ShowMissionVotingInterface()
        {
            EnableVotingBtns = true;
        }

        internal void ShowTeamVoteResult(Dictionary<Player, bool> voters, bool result)
        {
            EnableVotingBtns = false;
            EnableOKBtn = true;
            RoundBox = "";

            if (!result)
            {
                RestorePlayersToNormal();
                RoundBox += $"Team formation failed. {GameState.FailedVoteCount}/5 ";
            }

            foreach (Player p in voters.Keys)
            {
                List<User> userList = _as.UserList;
                RoundBox += "[" + p.Name + ": "; //TODO Make this prettier with a proper label
                if (voters[p])
                {
                    RoundBox += "Yes] ";
                }
                else
                {
                    RoundBox += "No] ";
                }
            }
        }

        internal void ResolveMissionResult()
        {
            ScoreNodeView scoreNode = ScoreDictionary[GameState.CurrentRound];
            scoreNode.ChangeState(GameState.MissionVictory);
            GameState.CurrentRound++;
            if (GameState.MissionVictory)
            {
                RoundBox = "Mission successful";
            }
            else
            {
                RoundBox = "Mission failed";
            }
            EnableOKBtn = true;
            EnableVotingBtns = false;
        }

        internal void ShowEndGameResults()
        {
            EnableOKBtn = false;
            EnableVotingBtns = false;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var spiesIDs = GameState.Players.FindAll((p) => p.Faction is SpiesFaction)
                           .Select((p, i) => p.UserID);
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
            RoundBox = GameState.Winner.Name + " victory!";
        }

        public void OnGameStateUpdate(GameState state)
        {
            GameState = state;
            switch (GameState.CurrentPhase)
            {
                case Phase.Setup:
                    PrepareAvatars();
                    RoundBox = $"You are a member of the {CurrentPlayer.Faction.Name} faction";
                    break;
                
                case Phase.TeamPicking:
                    if (CurrentPlayer == GameState.CurrentLeader)
                    {
                        SelectablePlayers = GameState.NextTeamSize;
                        ChangeOKBtnState(true);
                        RoundBox = $"Pick {GameState.NextTeamSize} players for the next mission";
                    }
                    else
                    {
                        ChangeOKBtnState(false);
                        RoundBox = $"{GameState.CurrentLeader.Name} is picking " +
                        $"{GameState.NextTeamSize} players for the next mission";
                    }
                    break;
                
                case Phase.TeamVoting:
                    TeamPickedUsersIDs = GameState.CurrentTeam.Select((p, i) => p.UserID).ToList();
                    ShowSelectedPlayers();
                    RoundBox = $"{GameState.CurrentLeader.Name} picked the following team.";
                    ChangeOKBtnState(false);
                    ChangeVotingBtnsState(true);
                    if (CurrentPlayer.UserID == GameState.CurrentLeader.UserID)
                    {
                        SelectablePlayers = 0;
                    }
                    break;
                
                case Phase.FailedTeamFormation:  // TODO Add max failures handling
                    ShowTeamVoteResult(GameState.RevealedTeamVotes, false);
                    break;

                case Phase.MissionVoting:
                    ShowTeamVoteResult(GameState.RevealedTeamVotes, true);
                    break;
                
                case Phase.MissionVoteResult:
                    ResolveMissionResult();
                    RestorePlayersToNormal();
                    TeamPickedUsersIDs.Clear();
                    break;
                
                case Phase.GameEnd:
                    ShowEndGameResults();
                    break;
            }
        }

        private Player FindCurrentPlayer()
        {
            return GameState.Players.Find(x => x.UserID == _as.CurrentUser.ID.ToString());
        }
    }
}
