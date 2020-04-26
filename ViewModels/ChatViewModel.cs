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
            ChatMessageLog = new ObservableCollection <ChatMessageViewModel>();
        }

        string messageContent;
        string chatLog;

        EditMessageViewModel edmContent;
        

        ObservableCollection<ChatMessageViewModel> chatMessageLog;

        public ObservableCollection <ChatMessageViewModel> ChatMessageLog
        {
            get => chatMessageLog;
            set => this.RaiseAndSetIfChanged(ref chatMessageLog, value);
        }

        public string Log
        {
            get => chatLog;
            set => this.RaiseAndSetIfChanged(ref chatLog, value);
        }
        public string MessageContent {
            get => messageContent;
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }
        public EditMessageViewModel EDMContent {
            get => edmContent;
            set => this.RaiseAndSetIfChanged(ref edmContent, value);
        }

        public void OnSendClicked()
        {
            if(MessageContent!=null && MessageContent.Trim() != ""){
                LocalUser user = GameState.GetGameState().CurrentUser;
                MessageContent = user.Name + ": " + MessageContent;
                Message chatText = new Message(MessageContent, JeffistanceFlags.Chat, JeffistanceFlags.Broadcast);
                user.Send(chatText);
                this.MessageContent = "";
            }
        }

        public void WriteLineInLog(string msg)
        {
            ChatMessageViewModel c = new ChatMessageViewModel(ChatMessageLog.Count.ToString(),  msg, this);
            Dispatcher.UIThread.Post(()=> this.ChatMessageLog.Add(c));
        }

        public void RemoveChatMessage(ChatMessageViewModel message)
        {
            Dispatcher.UIThread.Post(()=> this.ChatMessageLog.Remove(message));
        }
        
    }
}