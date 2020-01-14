using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Input;


namespace Jeffistance.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NetworkTest(true);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        User User;
        public void NetworkTest(bool host=false)
        {
            string ip;
            if(host)
            {
                ServerConnection server = new ServerConnection();
                ip = NetworkUtilities.GetLocalIPAddress();
            }
            else
            {
                ip = "176.78.147.48";
            }
            User = new User(new ClientConnection(ip));
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            ClientConnection cc = (ClientConnection) User.Connection;
            cc.Send("button clicked epicly");
        }

        private void OnPointerEnter(object sender, PointerEventArgs e)
        {

        }

        private void OnPointerLeave(object sender, PointerEventArgs e)
        {
        }
    }
}