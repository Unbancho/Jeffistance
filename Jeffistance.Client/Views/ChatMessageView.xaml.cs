using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
{
    public class ChatMessageView : UserControl
    {
        public TextBox UsernameTextBox => this.FindControl<TextBox>("UsernameBox");

        public ChatMessageView()
        {
            this.InitializeComponent();
            UsernameTextBox.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}