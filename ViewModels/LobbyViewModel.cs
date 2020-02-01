namespace Jeffistance.ViewModels
{
    public class LobbyViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        public LobbyViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
        }
    }
}
