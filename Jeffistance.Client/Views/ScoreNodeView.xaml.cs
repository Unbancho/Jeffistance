using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

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
            nodeImage.Source = new Bitmap("Jeffistance.Client\\Assets\\Skullbisu.png");
            State = ScoreState.NoResult;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ChangeState(bool roundResult)
        {
            if(roundResult)
            {
                State = ScoreState.JeffistanceVictory;
                nodeImage.Source = new Bitmap("Jeffistance.Client\\Assets\\Sharkbisu.png");
            }
            else
            {
               State = ScoreState.JeffistanceVictory;
               nodeImage.Source = new Bitmap("Jeffistance.Client\\Assets\\Vorebisu.png"); 
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