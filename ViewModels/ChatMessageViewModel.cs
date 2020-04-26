using System.Reactive.Linq;
using ReactiveUI;
using System;

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
        public void OnEditClicked()
        {
            var emvm = new EditMessageViewModel(Content, Parent);

            Observable.Merge(emvm.OnOkButton, emvm.OnCancelClicked.Select(_ => (ChatMessageViewModel)null))
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

            Parent.ChatContent = emvm;
            //Content = this.Content + " (edited)";
        }
        
    }
}
