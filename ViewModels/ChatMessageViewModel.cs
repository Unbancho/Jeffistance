using ReactiveUI;

namespace Jeffistance.ViewModels
{
    public class ChatMessageViewModel : ViewModelBase
    {
        public ChatMessageViewModel(string id, string content)
        {
            this.id = id;
            this.Content = content;
        }

        string id;

        string content;

        public string Content
        {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void OnDeleteClicked()
        {
        }
        public void OnEditClicked()
        {
        }
        
    }
}
