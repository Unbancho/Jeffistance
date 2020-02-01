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
        User currentUser;

        public User CurrentUser
        {
            get => currentUser;
            set => this.RaiseAndSetIfChanged(ref currentUser, value);
        }

        //TODO Actual port validation
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

        public ReactiveCommand<Unit, Unit> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public HostMenuViewModel(MainWindowViewModel parent)
        {
            this.parent = parent;
            var okEnabled = this.WhenAnyValue(
                x => x.Port,
                x => x != "-1"
            );
            var cancelEnabled = this.WhenAnyValue(
                x => x.CurrentUser,
                selector: x => x is null
            );

            Ok = ReactiveCommand.Create(
                () => {
                    Console.WriteLine($"Hosting on {port}");
                    Host();},
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => {parent.Content = new MainMenuViewModel(parent);},
                cancelEnabled
            );
        }

        public void Host()
        {
            if(CurrentUser == null)
            {
                CurrentUser = new Host(port);
            }
            CurrentUser.Connection.Send("button clicked epicly");
        }
    }
}