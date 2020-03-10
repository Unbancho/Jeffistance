using System;
using System.Reactive;
using ReactiveUI;
using Jeffistance.Models;

namespace Jeffistance.ViewModels
{
    public class HostMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        int port = User.DEFAULT_PORT;
        
        //TODO Actual port validation
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

        public string Username {get; set;}

        public ReactiveCommand<Unit, Unit> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public HostMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            var okEnabled = this.WhenAnyValue(
                x => x.Port,
                x => x != "-1"
            );

            Ok = ReactiveCommand.Create(
                () => {
                    Console.WriteLine($"Hosting on {port}");
                    Host();},
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => {parent.Content = new MainMenuViewModel(parent);}
            );
        }

        public void Host()
        {
            GameState gs = GameState.GetGameState();
            gs.CurrentUser = new Host(Username, port);
            parent.Content = new LobbyViewModel(parent);
            gs.CurrentUser.CurrentWindow = parent.Content;
        }
    }
}