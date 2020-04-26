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
    public class ChatMessageListViewModel : ViewModelBase
    {
        public ChatMessageListViewModel()
        {
            ChatMessageLog = new ObservableCollection <ChatMessageViewModel>();
        }

        ObservableCollection<ChatMessageViewModel> chatMessageLog;

        public ObservableCollection <ChatMessageViewModel> ChatMessageLog
        {
            get => chatMessageLog;
            set => this.RaiseAndSetIfChanged(ref chatMessageLog, value);
        }
        
    }
}