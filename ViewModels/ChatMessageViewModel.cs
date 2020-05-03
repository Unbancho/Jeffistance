using System.Reactive.Linq;
using ReactiveUI;
using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Reactive;
using Jeffistance.Models;

namespace Jeffistance.ViewModels
{
    public class ChatMessageViewModel : ViewModelBase
    {
        public ChatMessageViewModel(Guid id, string content, ChatViewModel parent)
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

        Guid id;

        string content;

        string username;

        ChatViewModel parent;

        bool edited {get; set;}

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
            parent.RemoveChatMessage(this);
        }

        public ReactiveCommand<Control, Unit> OnEditClicked { get; }
        
        public void OnEditClickedMethod(Control testControl)
        {
            var emvm = new EditMessageViewModel(id, Content, Parent, Username);
            Window editWindow = CreateEditWindow(emvm);
            editWindow.ShowDialog((Window)testControl.GetVisualRoot());
            Observable.Merge(emvm.OnOkClicked, emvm.OnCancelClicked.Select(_ => (ChatMessageViewModel)null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        ChatMessageViewModel message = (ChatMessageViewModel) model;
                        edited = true;
                        Content = message.content;
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
