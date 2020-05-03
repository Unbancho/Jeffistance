using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Models;
using System;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ModusOperandi.Messaging;
using Jeffistance.Services.MessageProcessing;

namespace Jeffistance.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel()
        {
            
           Log = new ObservableCollection <ChatMessageViewModel>();
        }

        ObservableCollection<ChatMessageViewModel> log;

        public ObservableCollection <ChatMessageViewModel> Log
        {
            get => log;
            set => this.RaiseAndSetIfChanged(ref log, value);
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
                Message chatText = new Message(MessageContent, JeffistanceFlags.Chat, JeffistanceFlags.Broadcast);
                chatText.Sender = user.Name;
                user.Connection.Send(chatText);
                this.MessageContent = "";
            }
        }


        public void WriteLineInLog(string msg, string username)
        {
            Guid guidId = Guid.NewGuid();
            ChatMessageViewModel c = new ChatMessageViewModel(username, guidId,  msg, this);
            Dispatcher.UIThread.Post(()=> Log.Add(c));
        }

        public void RemoveChatMessage(ChatMessageViewModel message)
        {
            Dispatcher.UIThread.Post(()=> Log.Remove(message));
        }
        
    }
}