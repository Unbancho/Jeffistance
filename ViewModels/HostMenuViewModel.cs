using System;
using System.Reactive;
using ReactiveUI;

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
                    Console.WriteLine($"Would host with {port}");
                    NetworkTest(true);},
                okEnabled
            );
            Cancel = ReactiveCommand.Create(
                () => {parent.Content = new MainMenuViewModel(parent);},
                cancelEnabled
            );
        }

        public void NetworkTest(bool host=false)
        {
            if(CurrentUser == null)
            {
                CurrentUser = host ? new Host(port) : new User();
                if(!host)
                {
                    CurrentUser.Connect("176.78.147.48");
                }
            }
            CurrentUser.Connection.Send("button clicked epicly");
        }
    }
}