using ReactiveUI;
using System.Reactive;
using System;
using Avalonia.Controls;

namespace Jeffistance.ViewModels
{
    public class EditMessageViewModel : ViewModelBase
    {
        private string _username;
        private Guid _id;
        private ChatViewModel _parent;
        public EditMessageViewModel(Guid id, string messageText, ChatViewModel parent, string username)
        {
            _username = username;
            _parent = parent;
            _id = id;
            MessageContent = messageText;
            var okEnabled = this.WhenAnyValue(
                x => x.MessageContent,
                x => !string.IsNullOrWhiteSpace(x));

            OnOkClicked = ReactiveCommand.Create(OnOkClickedMethod, okEnabled);

            OnCancelClicked = ReactiveCommand.Create(() => { });
        }

        string messageContent;
       
        public string MessageContent {
            get => messageContent;
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }

        public ReactiveCommand<Unit, ChatMessageViewModel> OnOkClicked { get; }
        public ReactiveCommand<Unit, Unit> OnCancelClicked { get; }

        public ChatMessageViewModel OnOkClickedMethod(){
            
            return new ChatMessageViewModel (_username, _id, MessageContent, _parent);
        }

    }
}