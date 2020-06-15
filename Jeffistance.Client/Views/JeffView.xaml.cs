using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
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