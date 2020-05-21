using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
{
    public class PlayerAreaView : UserControl
    {
        public PlayerAreaView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}