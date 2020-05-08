using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using ReactiveUI;


namespace Jeffistance.Client.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        public string Greeting => "0.01";
        public string TheTruth => "Jeff ungae";

        public MainMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
        }

        private void OnHostButtonClick()
        {
            parent.Content = new HostMenuViewModel(parent);
        }

        private void OnJoinButtonClick()
        {
            parent.Content = new JoinMenuViewModel(parent);
        }

        public void AutoHost()
        {
            OnHostButtonClick();
            ((HostMenuViewModel) parent.Content).Host();
        }

        public void AutoJoin()
        {
            OnJoinButtonClick();
            ((JoinMenuViewModel) parent.Content).Username = "DebugBoy";
            ((JoinMenuViewModel) parent.Content).Join();
        }
    }
}
