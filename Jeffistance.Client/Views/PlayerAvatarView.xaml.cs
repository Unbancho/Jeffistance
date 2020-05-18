using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
{
    public class PlayerAvatarView : UserControl
    {
        public PlayerAvatarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}