using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Client.Models;
using System;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ModusOperandi.Messaging;
using Jeffistance.Common.Services.MessageProcessing;
using Jeffistance.Common.Models;

namespace Jeffistance.Client.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel()
        {
           ChatMessageLog = new ObservableCollection <ChatMessageViewModel>();
        }

        string messageContent;

        public string MessageContent {
            get => messageContent;
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }

        public ObservableCollection <ChatMessageViewModel> chatMessageLog;

        public ObservableCollection <ChatMessageViewModel> ChatMessageLog
        {
            get => chatMessageLog;
            set => this.RaiseAndSetIfChanged(ref chatMessageLog, value);
        }

        public void OnSendClicked()
        {
            if (MessageContent != null && MessageContent.Trim() != "")
            {
                LocalUser user = AppState.GetAppState().CurrentUser;
                MessageContent = user.Name + ": " + MessageContent;
                Message chatText = new Message(MessageContent, JeffistanceFlags.Chat);
                //chatText.Sender = user.Name; //TODO Remove after sender is automatically setted in MO's constructor
                user.Send(chatText);
                this.MessageContent = "";
            }
        }
         public void WriteLineInLog(string msg, string username)
        {
            ChatMessageViewModel c = new ChatMessageViewModel(Guid.NewGuid(),  msg, this, username);
            Dispatcher.UIThread.Post(()=> this.ChatMessageLog.Add(c));
        }

        public void RemoveChatMessage(ChatMessageViewModel message)
        {
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Remove(message));
        }
        
    }
}