using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class JoinMenuView : UserControl
    {
        public TextBox UsernameTextBox => this.FindControl<TextBox>("UsernameTextBox");

        public JoinMenuView()
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