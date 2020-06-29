using System;
using System.Reactive;
using ReactiveUI;
using Jeffistance.Client.Models;
using Jeffistance.JeffServer.Models;
using Jeffistance.Client.Services.MessageProcessing;
using Jeffistance.Common.ExtensionMethods;
using Jeffistance.Common.Services.IoC;
using Microsoft.Extensions.Logging;

namespace Jeffistance.Client.ViewModels
{
    public class HostMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        int port = 7700;
        private ILogger _logger;
        
        public string Port
        {
            get => port.ToString();
            set {
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

        public string Username {get; set;}

        public ReactiveCommand<Unit, Unit> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public HostMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            this._logger = IoCManager.GetClientLogger();

            var okEnabled = this.WhenAnyValue(
                x => x.Port,
                x => x != "-1"
            );

            Ok = ReactiveCommand.Create(
                () => {
                    _logger.LogInformation($"Hosting on {port}");
                    Host();},
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => {parent.Content = new MainMenuViewModel(parent);}
            );
        }

        public void Host()
        {
            AppState gs = AppState.GetAppState();
            Server server = new Server();
            gs.Server = server;
            server.Run(port);
            gs.CurrentUser = server.Host;
            server.ConnectHost(Username, new ClientMessageProcessor());
            LobbyViewModel lobby = new LobbyViewModel(parent);
            parent.Content = lobby;
            gs.CurrentLobby = lobby;
            gs.CurrentWindow = parent.Content;
            _logger.LogInformation("Successfully hosted server.");
        }
    }
}