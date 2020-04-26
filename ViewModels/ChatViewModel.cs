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
            ChatContent = List = new ChatMessageListViewModel();
        }

        string messageContent;
        string chatLog;

        ViewModelBase chatContent;

        public ViewModelBase ChatContent
        {
            get => chatContent;
            set => this.RaiseAndSetIfChanged(ref chatContent, value);
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

        public ChatMessageListViewModel list;

        public ChatMessageListViewModel List {
            get => list;
            set => this.RaiseAndSetIfChanged(ref list, value);
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

        internal void RestoreList()
        {
            ChatContent = List;
        }

        public void WriteLineInLog(string msg)
        {
            ChatMessageListViewModel ChatMessageLog = (ChatMessageListViewModel) ChatContent;
            ChatMessageViewModel c = new ChatMessageViewModel(ChatMessageLog.Log.Count.ToString(),  msg, this);
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Log.Add(c));
            ChatContent = List = ChatMessageLog;
        }

        public void RemoveChatMessage(ChatMessageViewModel message)
        {
            ChatMessageListViewModel ChatMessageLog = (ChatMessageListViewModel) ChatContent;
            Dispatcher.UIThread.Post(()=> ChatMessageLog.Log.Remove(message));
        }
        
    }
}