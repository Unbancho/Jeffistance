using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class LobbyView : UserControl
    {
        public LobbyView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}