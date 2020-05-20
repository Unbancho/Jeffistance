using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Jeffistance.Client.Models;
using Jeffistance.Client.Views;
using Jeffistance.Common.Models;
using ReactiveUI;

namespace Jeffistance.Client.ViewModels
{
    public class GameScreenViewModel : ViewModelBase, IChatView
    {
        private PlayerAreaViewModel _playerArea;
        private ChatViewModel _chatView;
        private StackPanel _scorePanel;
        //private Dictionary<int, ScoreNodeView> _scoreDictionary;

        public GameScreenViewModel()
        {
            PlayerArea = new PlayerAreaViewModel();
            //ScoreDictionary = new Dictionary<int, ScoreNodeView>();
            ChatView = new ChatViewModel();
            ScorePanel = new StackPanel();
            AppState appState = AppState.GetAppState();

            //Adding players
            List<User> userList = appState.UserList;
            foreach(User u in userList)
            {
                PlayerAvatarView pav = new PlayerAvatarView(u.Name, u.ID.ToString());
                pav.PointerPressed += onAvatarClicked;
                PlayerArea.CircularPanel.Children.Add(pav);
            }

            //Adding score nodes
            for (int index = 1; index <= 5; index++)
            {               
                ScoreNodeView snv = new ScoreNodeView();
                ScorePanel.Children.Add(snv);
                //ScoreDictionary.Add(index, snv);
            }
        }



        private void onAvatarClicked(object sender, PointerPressedEventArgs args)
        {
            PlayerAvatarView player = (PlayerAvatarView) sender;
            //player.avatar.Source =  new Bitmap("Jeffistance.Client\\Assets\\Vorebisu.png");;
            Console.WriteLine(player.UserId);
        }
        
        public StackPanel ScorePanel
        {
            get => _scorePanel;
            set => this.RaiseAndSetIfChanged(ref _scorePanel, value);
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

        public ChatViewModel ChatView
        {
            get => _chatView;
            set => this.RaiseAndSetIfChanged(ref _chatView, value);
        }
        

    }

}