using ReactiveUI;

//This class is not very necesary, but just in case we want to customize the player area further in the future im leaving it as is.
namespace Jeffistance.Client.ViewModels
{
    public class PlayerAreaViewModel : ViewModelBase
    {
        private CircularPanel _circularPanel;

        public CircularPanel CircularPanel
        {
            get => _circularPanel;
            set => this.RaiseAndSetIfChanged(ref _circularPanel, value);
        }

        public PlayerAreaViewModel()
        {
            CircularPanel = new CircularPanel();
        }
    }
}
