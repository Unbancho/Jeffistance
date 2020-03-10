using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jeffistance.Models;
using System;
using Avalonia.Controls;
using ReactiveUI;
using Jeffistance.Services.Messaging;
using Jeffistance.Services.MessageProcessing;

namespace Jeffistance.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ChatViewModel()
        {
        }

        string messageContent;
        string chatLog;

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
            User user = GameState.GetGameState().CurrentUser;
            MessageContent = user.Name + ": " + MessageContent;
            Message chatText = new Message(MessageContent, JeffistanceFlags.Chat, JeffistanceFlags.Broadcast);
            user.Send(chatText);
            this.MessageContent = "";
        }

        public void WriteLineInLog(string msg)
        {
            this.Log = this.Log + msg + "\n";
        }

    }
}