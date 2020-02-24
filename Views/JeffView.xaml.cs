using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance
{
    public class JeffView : UserControl
    {
        public JeffView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}