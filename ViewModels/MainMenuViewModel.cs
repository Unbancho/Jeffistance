using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using ReactiveUI;

namespace Jeffistance.ViewModels
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
    }
}
