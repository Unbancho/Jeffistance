using System.Collections.ObjectModel;
using Avalonia.Threading;
using ReactiveUI;
using Jeffistance.Client.Services;
using Jeffistance.Common.Services.IoC;
using System.Reactive;
using System.Collections.Generic;

namespace Jeffistance.Client.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel()
        {
            _chatManager = IoCManager.Resolve<IClientChatManager>();
            ChatMessageLog = new ObservableCollection <ChatMessageViewModel>();
            chatMessageDictionary = new Dictionary<string, ChatMessageViewModel>();
            AutoScrollToggled = true;
            ToggleAutoScroll = ReactiveCommand.Create(
                () => { AutoScrollToggled = !AutoScrollToggled; }
            );
        }

        private IClientChatManager _chatManager;

        private string _messageContent;

        public Dictionary<string, ChatMessageViewModel> chatMessageDictionary;

        private ObservableCollection<ChatMessageViewModel> _chatMessageLog;
        public ObservableCollection <ChatMessageViewModel> ChatMessageLog
        {
            get => _chatMessageLog;
            set => this.RaiseAndSetIfChanged(ref _chatMessageLog, value);
        }

        public string MessageContent {
            get => _messageContent;
            set => this.RaiseAndSetIfChanged(ref _messageContent, value);
        }

        private ChatMessageViewModel _selectedMessage;
        public ChatMessageViewModel SelectedMessage{
            get => _selectedMessage;
            set => this.RaiseAndSetIfChanged(ref _selectedMessage, value);
        }

        private bool _autoScrollToggled;
        public bool AutoScrollToggled{
            get => _autoScrollToggled;
            set => this.RaiseAndSetIfChanged(ref _autoScrollToggled, value);
        }

        public ReactiveCommand<Unit, Unit> ToggleAutoScroll { get; }

        public void OnSendClicked()
        {
            if (!string.IsNullOrWhiteSpace(MessageContent))
            {
                _chatManager.Send(MessageContent);
                MessageContent = "";
            }
        }
         public void WriteLineInLog(string msg, string username, string msgId)
        {
            var chatMessage = new ChatMessageViewModel(msgId, msg, this, username);
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Add(chatMessage));
            chatMessageDictionary.Add(chatMessage.id, chatMessage);
            if(AutoScrollToggled)
                ScrollToMessage(chatMessage);
        }

        private void ScrollToMessage(ChatMessageViewModel chatMessage, bool selectMessage=false)
        {
            SelectedMessage = chatMessage;
            if(!selectMessage)
                SelectedMessage = null;
        }


        public void DeleteMessage(string msgId)
        {
            ChatMessageViewModel cmvm = FindMessage(msgId);
            chatMessageDictionary.Remove(msgId);
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Remove(cmvm));
        }
        
        public void EditMessage(string msgId, string newText)
        {
            ChatMessageViewModel cmvm = FindMessage(msgId);
            cmvm.Content = newText + " (Edited)";
            cmvm.edited = true;
        }

        public ObservableCollection<ChatMessageViewModel> FindTextInMessage(string txt){
            ObservableCollection<ChatMessageViewModel> filteredMessages = new ObservableCollection <ChatMessageViewModel>();
            foreach(ChatMessageViewModel c in ChatMessageLog)
            {
                if(c.Content.Contains(txt))
                {
                   filteredMessages.Add(c);
                }
            }
            return filteredMessages;
        }

        public ChatMessageViewModel FindMessage(string id)
        {
            if(chatMessageDictionary.ContainsKey(id)){
                return chatMessageDictionary[id];
            }
            return null;
        }
    }

}