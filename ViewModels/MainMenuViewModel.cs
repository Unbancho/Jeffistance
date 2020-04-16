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

        public ReactiveCommand<Control, Unit> AboutMenu { get; } 

        public MainMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;

            AboutMenu = ReactiveCommand.Create<Control>(
                (control) => {
                    Window popupWindow = CreateAboutWindow();
                    popupWindow.ShowDialog((Window)control.GetVisualRoot());
                }
            );
        }

        private Window CreateAboutWindow()
        {
            Window window = new Window()
            {
                Title = "About",
                ShowInTaskbar = false,
                Height = 200,
                Width = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Content = new TextBlock {
                Text = "Jeffistance is a very epic app made by epic people for epic people.",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 24
            };
            
            return window;
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
