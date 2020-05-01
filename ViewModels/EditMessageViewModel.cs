using ReactiveUI;
using System.Reactive;
using System;

namespace Jeffistance.ViewModels
{
    public class EditMessageViewModel : ViewModelBase
    {
        public EditMessageViewModel(Guid id, string messageText, ChatViewModel parent, string username)
        {
            MessageContent = messageText;
            var okEnabled = this.WhenAnyValue(
                x => x.MessageContent,
                x => !string.IsNullOrWhiteSpace(x));

            OnOkClicked = ReactiveCommand.Create(
                () => new ChatMessageViewModel (username, id, MessageContent, parent),
                okEnabled);

            OnCancelClicked = ReactiveCommand.Create(() => { });
        }

        string messageContent;
       
        public string MessageContent {
            get => messageContent;
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }

        public ReactiveCommand<Unit, ChatMessageViewModel> OnOkClicked { get; }
        public ReactiveCommand<Unit, Unit> OnCancelClicked { get; }

    }
}