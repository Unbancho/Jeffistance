using ReactiveUI;
using Jeffistance.Client.Models;

namespace Jeffistance.Client.ViewModels
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
            switch (AppState.GetAppState().Arguments[0].ToLower())
            {
                case "host":
                    ((MainMenuViewModel) Content).AutoHost();
                    break;
                case "join":
                    ((MainMenuViewModel) Content).AutoJoin();
                    break;
            }
        }

    }
}
