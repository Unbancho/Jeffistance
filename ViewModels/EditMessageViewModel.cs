using ReactiveUI;
using System.Reactive;

namespace Jeffistance.ViewModels
{
    public class EditMessageViewModel : ViewModelBase
    {
        public EditMessageViewModel(string messageText, ChatViewModel parent)
        {
            MessageContent = messageText;
            var okEnabled = this.WhenAnyValue(
                x => x.MessageContent,
                x => !string.IsNullOrWhiteSpace(x));

            OnOkButton = ReactiveCommand.Create(
                () => new ChatMessageViewModel ("test", messageText, parent),
                okEnabled);

            OnCancelClicked = ReactiveCommand.Create(() => { });

        }

        string messageContent;
       
        public string MessageContent {
            get => messageContent;
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }

        public ReactiveCommand<Unit, ChatMessageViewModel> OnOkButton { get; }
        public ReactiveCommand<Unit, Unit> OnCancelClicked { get; }

    }
}