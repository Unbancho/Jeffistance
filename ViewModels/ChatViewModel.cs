using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Models;
using System;
using Avalonia.Controls;
using ReactiveUI;
using ModusOperandi.Messaging;
using Jeffistance.Services.MessageProcessing;

namespace Jeffistance.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel()
        {
            chatMessageLog = new ObservableCollection <ChatMessageViewModel>();
        }

        string messageContent;
        string chatLog;

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

        public void OnSendClicked()
        {
            if(MessageContent!=null && MessageContent.Trim() != ""){
                LocalUser user = GameState.GetGameState().CurrentUser;
                MessageContent = user.Name + ": " + MessageContent;
                ChatMessageViewModel c = new ChatMessageViewModel(ChatMessageLog.Count.ToString(),  MessageContent);
                this.ChatMessageLog.Add(c);

                Message chatText = new Message(MessageContent, JeffistanceFlags.Chat, JeffistanceFlags.Broadcast);
                user.Send(chatText);

                this.MessageContent = "";
            }
        }

        public void WriteLineInLog(string msg)
        {
            this.Log = this.Log + msg + "\n";
        }

    }
}