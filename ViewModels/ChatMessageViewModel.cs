using System.Reactive.Linq;
using ReactiveUI;
using System;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Jeffistance.ViewModels
{
    public class ChatMessageViewModel : ViewModelBase
    {
        public ChatMessageViewModel(string id, string content, ChatViewModel parent)
        {
            this.id = id;
            this.Content = content;
            this.Parent = parent;
        }

        string id;

        string content;

        ChatViewModel parent;

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

        public void OnDeleteClicked()
        {
            parent.RemoveChatMessage(this);
        }
        public void OnEditClicked(Control testControl)
        {
            var emvm = new EditMessageViewModel(id, Content, Parent);

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

            /* This instead of just setting the chat content

                    Window popupWindow = CreateEditWindow();
                    popupWindow.ShowDialog((Window)control.GetVisualRoot());

                    The only issue is, we need access to the main window, I did it by sending
                    the control from the XAML and then accessing its visual root so uhhhh
            */
            //Parent.ChatContent = emvm;
            Window editWindow = CreateEditWindow(emvm);
            editWindow.ShowDialog((Window)testControl.GetVisualRoot());
            //Content = this.Content + " (edited)";
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
