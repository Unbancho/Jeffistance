using System;
using System.Net;
using System.Reactive;
using ReactiveUI;
using Jeffistance.Models;
using ModusOperandi.Networking;

namespace Jeffistance.ViewModels
{
    public class JoinMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        int port = User.DEFAULT_PORT;
        string ipAddress = NetworkUtilities.GetLocalIPAddress();

        public string Username {get; set;}

        public string Port
        {
            get => port.ToString();
            set {
                if (Int32.TryParse(value, out int result) && result >= 0 && result <= 65535)
                {
                    this.RaiseAndSetIfChanged(ref port, result);
                }
                else
                {
                    this.RaiseAndSetIfChanged(ref port, -1);
                }
            }
        }

        public string IpAddress
        {
            get => ipAddress;
            set => this.RaiseAndSetIfChanged(ref ipAddress, value);
        }

        public ReactiveCommand<Unit, Unit> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public JoinMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;

            var okEnabled = this.WhenAnyValue(
                x => x.Port,
                x => x.IpAddress,
                (port, ip) => port != "-1" && IPAddress.TryParse(ip, out IPAddress _)
            );

            Ok = ReactiveCommand.Create(
                () => {
                    Console.WriteLine($"Joining {IpAddress}:{port}");
                    Join();},
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => {parent.Content = new MainMenuViewModel(parent);}
            );
        }

        public void Join()
        {
            GameState gs = GameState.GetGameState();
            gs.CurrentUser = new User(Username);
            gs.CurrentUser.Connect(IpAddress, port);
            parent.Content = new LobbyViewModel(parent);
            gs.CurrentWindow = parent.Content;
        }
    }
}