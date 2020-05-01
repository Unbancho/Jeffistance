using System.Collections.ObjectModel;
using ReactiveUI;

namespace Jeffistance.ViewModels
{
    public class ChatMessageListViewModel : ViewModelBase
    {
        public ChatMessageListViewModel()
        {
           Log = new ObservableCollection <ChatMessageViewModel>();
        }

        ObservableCollection<ChatMessageViewModel> log;

        public ObservableCollection <ChatMessageViewModel> Log
        {
            get => log;
            set => this.RaiseAndSetIfChanged(ref log, value);
        }
        
    }
}