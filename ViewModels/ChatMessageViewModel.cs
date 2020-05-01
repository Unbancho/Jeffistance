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
        public ChatMessageViewModel(string username, Guid id, string content, ChatViewModel parent)
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

            Observable.Merge(emvm.OnOkClicked, emvm.OnCancelClicked.Select(_ => (ChatMessageViewModel)null))
                .Take(1)
                .Subscribe(model =>
                {
                    if (model != null)
                    {
                        ChatMessageViewModel message = (ChatMessageViewModel) model;
                        Content = message.content + " (Edited)";
                    }

                    Parent.RestoreList();
                });
                    /*
                     This instead of just setting the chat content

                    Window popupWindow = CreateEditWindow();
                    popupWindow.ShowDialog((Window)control.GetVisualRoot());

                    The only issue is, we need access to the main window, I did it by sending
                    the control from the XAML and then accessing its visual root so uhhhh
                    */
            Window editWindow = CreateEditWindow(emvm);
            editWindow.ShowDialog((Window)testControl.GetVisualRoot());
        }

        private Window CreateEditWindow(ViewModelBase windowContent)
        {
            Window window = new Window()
            {
                Title = "Edit",
                ShowInTaskbar = false,
                Height = 200,
                Width = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Content = windowContent;
            
            return window;
        }
        
    }
}
