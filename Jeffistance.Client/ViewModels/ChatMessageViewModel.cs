using System.Reactive.Linq;
using ReactiveUI;
using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Reactive;
using Jeffistance.Client.Models;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services.IoC;
using Jeffistance.Common.Services;

namespace Jeffistance.Client.ViewModels
{
    public class ChatMessageViewModel : ViewModelBase
    {
        public ChatMessageViewModel(string id, string content, ChatViewModel parent, string username)
        {
            this.id = id;
            Content = content;
            Parent = parent;
            Username = username;
            IsAuthor = AppState.GetAppState().CurrentUser.Name.Equals(Username);

            var isAuthor = this.WhenAnyValue(
                x => x.IsAuthor,
                x => x == true);

            OnEditClicked = ReactiveCommand.Create<Control>(OnEditClickedMethod, isAuthor);
            OnDeleteClicked = ReactiveCommand.Create(OnDeleteClickedMethod, isAuthor);
        }

        public string id {get; set;}

        string content;

        string username;

        bool _isAuthor;

        ChatViewModel parent;

        public bool edited {get; set;}

        public bool IsAuthor
        {
            get => _isAuthor;
            set => this.RaiseAndSetIfChanged(ref _isAuthor, value);
        }
        public string Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        public string Content
        {
            get => content;

            set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public ChatViewModel Parent
        {
            get => parent;
            set => this.RaiseAndSetIfChanged(ref parent, value);
        }

        public ReactiveCommand<Unit, Unit> OnDeleteClicked { get; }
        public void OnDeleteClickedMethod()
        {
            LocalUser user = AppState.GetAppState().CurrentUser;
            var messageFactory = IoCManager.Resolve<IClientMessageFactory>();

            var chatMessage = messageFactory.MakeDeleteChatMessage(id);
            user.Send(chatMessage);
        }

        public ReactiveCommand<Control, Unit> OnEditClicked { get; }
        
        public void OnEditClickedMethod(Control testControl)
        {
            var emvm = new EditMessageViewModel(id, edited?Content.Substring(0, Content.Length - 9): Content, Parent, Username);
            Window editWindow = CreateEditWindow(emvm);
            editWindow.ShowDialog((Window)testControl.GetVisualRoot());
            Observable.Merge(emvm.OnOkClicked, emvm.OnCancelClicked.Select(_ => (ChatMessageViewModel)null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        ChatMessageViewModel message = (ChatMessageViewModel) model;
                        Content = message.content;

                        LocalUser user = AppState.GetAppState().CurrentUser;
                        var messageFactory = IoCManager.Resolve<IClientMessageFactory>();

                        var chatMessage = messageFactory.MakeEditChatMessage(Content, model.id);
                        user.Send(chatMessage);
                    }
                    editWindow.Close();
                });
        }

        private Window CreateEditWindow(ViewModelBase windowContent)
        {
            Window window = new Window()
            {
                Title = "Edit",
                ShowInTaskbar = false,
                Height = 100,
                Width = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Content = windowContent;
            
            return window;
        }
        
    }
}
