using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class MainMenuView : UserControl
    {
        public MainMenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}