using System.Reactive.Linq;
using ReactiveUI;
using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Reactive;
using Jeffistance.Client.Models;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;

namespace Jeffistance.Client.ViewModels
{
    public class ChatMessageViewModel : ViewModelBase
    {
        public ChatMessageViewModel(string id, string content, ChatViewModel parent, string username)
        {
            this.id = id;
            this.Content = content;
            this.Parent = parent;
            this.Username = username;

            var isAuthor = this.WhenAnyValue(
                x => x.Username,
                x => x == AppState.GetAppState().CurrentUser.Name);

            OnEditClicked = ReactiveCommand.Create<Control>(OnEditClickedMethod, isAuthor);
            OnDeleteClicked = ReactiveCommand.Create(OnDeleteClickedMethod, isAuthor);
        }

        public string id {get; set;}

        string content;

        string username;

        ChatViewModel parent;

        public bool edited {get; set;}

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
            Message chatMessage = new Message(Content, JeffistanceFlags.DeleteChatMessage);
            chatMessage["MessageID"] = id.ToString();
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
                        //edited = true;
                        Content = message.content;

                        LocalUser user = AppState.GetAppState().CurrentUser;
                        Message chatMessage = new Message(Content, JeffistanceFlags.EditChatMessage);
                        chatMessage["MessageID"] = model.id.ToString();
                        chatMessage["NewText"] = Content;
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
