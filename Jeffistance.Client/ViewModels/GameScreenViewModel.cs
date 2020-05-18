using Avalonia.Controls;
using Jeffistance.Client.Views;
using ReactiveUI;

namespace Jeffistance.Client.ViewModels
{
    public class GameScreenViewModel : ViewModelBase, IChatView
    {
        PlayerAreaViewModel gameScreen;
        ChatViewModel chatView;
        private StackPanel score;

        public StackPanel Score
        {
            get => score;
            set => this.RaiseAndSetIfChanged(ref score, value);
        }       
        public GameScreenViewModel()
        {
            GameScreen = new PlayerAreaViewModel();
            ChatView = new ChatViewModel();
            Score = new StackPanel();
            for (int index = 1; index <= 5; index++)
            {               
                ScoreNodeView snv = new ScoreNodeView();
                Score.Children.Add(snv);
            }
        }

         public PlayerAreaViewModel GameScreen
        {
            get => gameScreen;
            set => this.RaiseAndSetIfChanged(ref gameScreen, value);
        }

        public ChatViewModel ChatView
        {
            get => chatView;
            set => this.RaiseAndSetIfChanged(ref chatView, value);
        }

    }

}