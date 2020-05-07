using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class HostMenuView : UserControl
    {
        public TextBox UsernameTextBox => this.FindControl<TextBox>("UsernameTextBox");

        public HostMenuView()
        {
            InitializeComponent();
            UsernameTextBox.Initialized += (sender, args) => UsernameTextBox.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}