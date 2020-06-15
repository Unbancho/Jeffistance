using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
{
    public class GameScreenView : UserControl
    {
        public GameScreenView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}