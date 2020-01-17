using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System.ComponentModel;

namespace Jeffistance.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Closing += OnWindowClosing;
        }

        User User;
        public void NetworkTest(Button btn, bool host=false)
        {
            if(User == null)
            {
                btn.Content = "PING PEOPLE";
                User = host ? new Host() : new User();
                if(!host)
                {
                    User.Connect("176.78.147.48");
                }
            }
            User.Connection.Send("button clicked epicly");
        }

        private void OnHostButtonClick(object sender, RoutedEventArgs e)
        {
            NetworkTest((Button)sender, true);
        }

        private void OnJoinButtonClick(object sender, RoutedEventArgs e)
        {
            NetworkTest((Button)sender);
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if(User != null)
                User.Disconnect();
        }
    }
}