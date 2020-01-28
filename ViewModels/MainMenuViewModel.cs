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

        private void OnHostButtonClick()
        {
            parent.Content = new HostMenuViewModel(parent);
        }

        private void OnJoinButtonClick()
        {
            parent.Content = new JoinMenuViewModel();
        }
    }
}
