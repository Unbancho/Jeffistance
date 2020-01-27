using Avalonia.Controls;

namespace Jeffistance.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        public string Greeting => "Casca gae";
        public string TheTruth => "Jeff ungae";

        public MainMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
        }

        User User;
        public void NetworkTest(bool host=false)
        {
            if(User == null)
            {
                User = host ? new Host() : new User();
                if(!host)
                {
                    User.Connect("176.78.147.48");
                }
            }
            User.Connection.Send("button clicked epicly");
        }

        private void OnHostButtonClick()
        {
            parent.Content = new HostMenuViewModel();
            NetworkTest(true);
        }

        private void OnJoinButtonClick()
        {
            parent.Content = new JoinMenuViewModel();
            // NetworkTest();
        }
    }
}
