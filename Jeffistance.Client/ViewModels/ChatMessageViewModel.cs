using System;
using ReactiveUI;

namespace Jeffistance.ViewModels
{
    public class ChatMessageViewModel : ViewModelBase
    {
        public ChatMessageViewModel(Guid id, string content, ChatViewModel parent)
        {
            this.id = id;
            this.Content = content;
            this.Parent = parent;
        }

        Guid id;

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
            
        }
        
    }
}
