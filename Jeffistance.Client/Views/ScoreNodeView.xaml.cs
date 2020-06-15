using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Jeffistance.Client.Views
{
    public class ScoreNodeView : UserControl
    {
        
        private Image nodeImage;
        
        public ScoreState State;
        public ScoreNodeView()
        {
            InitializeComponent();
            nodeImage = this.FindControl<Image>("nodeImage");
            State = ScoreState.NoResult;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ChangeState(bool roundResult)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            if(roundResult)
            {
                State = ScoreState.JeffistanceVictory;
                Uri uri = new Uri("avares://Jeffistance.Client/Assets/Sharkbisu.png");
                nodeImage.Source = new Bitmap(assets.Open(uri));
            }
            else
            {
                State = ScoreState.JeffistanceVictory;
                Uri uri = new Uri("avares://Jeffistance.Client/Assets/Vorebisu.png");
                nodeImage.Source = new Bitmap(assets.Open(uri));
            }
        }

        
    public enum ScoreState
    {
        NoResult,
        JeffistanceVictory,
        SpyVictory
    }
    }
}