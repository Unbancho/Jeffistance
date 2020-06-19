using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Jeffistance.Common.AvaloniaTools;

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

        public void ChangeState(bool isJeffistanceVictory)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            if(isJeffistanceVictory)
            {
                State = ScoreState.JeffistanceVictory;
                nodeImage.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Sharkbisu.png");
            }
            else
            {
                State = ScoreState.SpyVictory;
                nodeImage.Source = AvaloniaTools.GetImageFromResources("Jeffistance.Client", "Vorebisu.png");
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