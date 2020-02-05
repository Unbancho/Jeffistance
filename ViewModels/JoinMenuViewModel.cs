using System;
using System.Reactive;
using ReactiveUI;
using Jeffistance.Models;
using Jeffistance.Services;

namespace Jeffistance.ViewModels
{
    public class JoinMenuViewModel : ViewModelBase
    {
        MainWindowViewModel parent;
        User currentUser;
        int port = User.DEFAULT_PORT;
        string ipAddress = NetworkUtilities.GetLocalIPAddress();

        public string Port
        {
            get => port.ToString();
            set {
                if (Int32.TryParse(value, out int result))
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

        public User CurrentUser
        {
            get => currentUser;
            set => this.RaiseAndSetIfChanged(ref currentUser, value);
        }

        public ReactiveCommand<Unit, Unit> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public JoinMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;

            var okEnabled = this.WhenAnyValue(
                x => x.Port,
                x => x.IpAddress,
                (port, ip) => port != "-1" && !string.IsNullOrWhiteSpace(ip)
            );
            var cancelEnabled = this.WhenAnyValue(
                x => x.CurrentUser,
                selector: x => x is null
            );

            Ok = ReactiveCommand.Create(
                () => {
                    Console.WriteLine($"Joining {IpAddress}:{port}");
                    Join();},
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => {parent.Content = new MainMenuViewModel(parent);},
                cancelEnabled
            );
        }

        public void Join()
        {
            if(CurrentUser == null)
            {
                CurrentUser = new User();
                CurrentUser.Connect(IpAddress, port);
            }
            CurrentUser.Connection.Send("help");
        }
    }
}