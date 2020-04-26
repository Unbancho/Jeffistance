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