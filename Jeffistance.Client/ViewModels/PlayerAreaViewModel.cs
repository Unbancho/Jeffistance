using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using ReactiveUI;
using System;
using System.Linq;
using System.ComponentModel;
using Avalonia.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Jeffistance.Client.Models;
using Jeffistance.Common.Models;
using Jeffistance.Common.Services.MessageProcessing;
using ModusOperandi.Messaging;
using Avalonia;
using Avalonia.Media.Imaging;

namespace Jeffistance.Client.ViewModels
{
    public class PlayerAreaViewModel : ViewModelBase
    {
        private ObservableCollection<Image> players;

        public ObservableCollection <Image> Players
        {
            get => players;
            set => this.RaiseAndSetIfChanged(ref players, value);
        }

        private CircularPanel circularPanel;

        public CircularPanel CircularPanel
        {
            get => circularPanel;
            set => this.RaiseAndSetIfChanged(ref circularPanel, value);
        }

        public PlayerAreaViewModel()
        {
            CircularPanel = new CircularPanel();
            Players = new ObservableCollection<Image>();
            Image im = new Image();
            im.Source = new Bitmap("Jeffistance.Client\\Assets\\Spy.png");
            im.Width=150;
            im.Height=150;
            Image im2 = new Image();
            im2.Source = new Bitmap("Jeffistance.Client\\Assets\\Spy.png");
            im2.Width=150;
            im2.Height=150;
            CircularPanel.Children.Add(im);
            CircularPanel.Children.Add(im2);
        }
    }
}
