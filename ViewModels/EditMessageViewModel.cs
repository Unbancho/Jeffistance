using ReactiveUI;
using System.Reactive;

namespace Jeffistance.ViewModels
{
    public class EditMessageViewModel : ViewModelBase
    {
        public EditMessageViewModel(string id, string messageText, ChatViewModel parent)
        {
            MessageContent = messageText;
            var okEnabled = this.WhenAnyValue(
                x => x.MessageContent,
                x => !string.IsNullOrWhiteSpace(x));

            OnOkClicked = ReactiveCommand.Create(
                () => new ChatMessageViewModel (id, MessageContent, parent),
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