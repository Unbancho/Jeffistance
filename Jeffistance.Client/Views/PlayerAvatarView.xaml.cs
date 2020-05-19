using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
{
    public class PlayerAvatarView : UserControl
    {
        public Image Avatar;
        public TextBox Username;

        public bool Selected;
        public string UserId;

        public ScoreState State;

        public PlayerAvatarView()
        {
        }
        
        public PlayerAvatarView(string playerName, string userId)
        {
            InitializeComponent();
            Avatar = this.FindControl<Image>("Avatar");
            Username = this.FindControl<TextBox>("Username");
            State = ScoreState.NoResult;
            Username.Text = playerName;
            UserId = userId;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public enum ScoreState
    {
        NoResult,
        JeffistanceVictory,
        SpyVictory
    }
}