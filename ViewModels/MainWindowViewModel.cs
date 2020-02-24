using ReactiveUI;

namespace Jeffistance.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public ViewModelBase Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }
        
        public MainWindowViewModel()
        {
            Content = new MainMenuViewModel(this);
        }
    }
}
