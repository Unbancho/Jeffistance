using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class HostMenuView : UserControl
    {
        public HostMenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}