using ReactiveUI;
using System.Reactive;

namespace Jeffistance.ViewModels
{
    public class EditMessageViewModel : ViewModelBase
    {
        public EditMessageViewModel(string messageText, ChatViewModel parent)
        {
            OnOkButton = ReactiveCommand.Create(
                () => new ChatMessageViewModel ("test", messageText, parent));
            OnCancelClicked = ReactiveCommand.Create(() => { });
            this.MessageContent = messageText;
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