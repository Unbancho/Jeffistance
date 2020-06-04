using System;
using System.Net;
using System.Reactive;
using ReactiveUI;
using Jeffistance.Client.Models;
using Jeffistance.Common.Models;
using ModusOperandi.Networking;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Client.Services.MessageProcessing;
using Jeffistance.Common.ExtensionMethods;

namespace Jeffistance.Client.ViewModels
{
    public class JoinMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        int port = 7700;
        string ipAddress = NetworkUtilities.GetLocalIPAddress();
        private string username;

        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        public string Port
        {
            get => port.ToString();
            set
            {
                if (Int32.TryParse(value, out int result) && result.IsValidPort())
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
                x => x.Username,
                (port, ip, u) =>
                port != "-1" && IPAddress.TryParse(ip, out IPAddress _) && !string.IsNullOrWhiteSpace(u)
            );

            Ok = ReactiveCommand.Create(
                () =>
                {
                    Console.WriteLine($"Joining {IpAddress}:{port}");
                    Join();
                },
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => { parent.Content = new MainMenuViewModel(parent); }
            );
        }

        public void Join()
        {
            AppState appState = AppState.GetAppState();
            appState.CurrentUser = new LocalUser(Username);
            appState.CurrentUser.Connect(IpAddress, port);
            appState.CurrentUser.AttachMessageHandler(new MessageHandler(new ClientMessageProcessor(), appState.CurrentUser.Connection));
            appState.CurrentUser.GreetServer();
            LobbyViewModel lobby = new LobbyViewModel(parent);
            parent.Content = lobby;
            appState.CurrentLobby = lobby;
            appState.CurrentWindow = parent.Content;
        }
    }
}