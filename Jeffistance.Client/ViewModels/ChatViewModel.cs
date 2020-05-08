using System.Collections.ObjectModel;
using Jeffistance.Client.Models;
using System;
using Avalonia.Threading;
using ReactiveUI;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;
using System.Reactive;

namespace Jeffistance.Client.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel()
        {
            ChatMessageLog = new ObservableCollection <ChatMessageViewModel>();
            AutoScrollToggled = true;
            ToggleAutoScroll = ReactiveCommand.Create(
                () => { AutoScrollToggled = !AutoScrollToggled; }
            );
        }

        private string _messageContent;
        private string _chatLog;

        private ObservableCollection<ChatMessageViewModel> _chatMessageLog;
        public ObservableCollection <ChatMessageViewModel> ChatMessageLog
        {
            get => _chatMessageLog;
            set => this.RaiseAndSetIfChanged(ref _chatMessageLog, value);
        }

        public string Log
        {
            get => _chatLog;
            set => this.RaiseAndSetIfChanged(ref _chatLog, value);
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
            if (MessageContent != null && MessageContent.Trim() != "")
            {
                LocalUser user = AppState.GetAppState().CurrentUser;
                Message chatMessage = new Message($"{user.Name}: {MessageContent}", JeffistanceFlags.Chat);
                user.Send(chatMessage);
                MessageContent = "";
            }
        }

        public void WriteLineInLog(string msg)
        {
            var chatMessage = new ChatMessageViewModel(Guid.NewGuid(),  msg, this);
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Add(chatMessage));
            if(AutoScrollToggled)
                ScrollToMessage(chatMessage);
        }

        private void ScrollToMessage(ChatMessageViewModel chatMessage, bool selectMessage=false)
        {
            SelectedMessage = chatMessage;
            if(!selectMessage)
                SelectedMessage = null;
        }

        public void RemoveChatMessage(ChatMessageViewModel message)
        {
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Remove(message));
        }

    }

}