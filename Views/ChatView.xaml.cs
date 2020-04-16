using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class ChatView : UserControl
    {
        public TextBox MessageTextBox => this.FindControl<TextBox>("MessageTextBox");

        public ChatView()
        {
            this.InitializeComponent();
            MessageTextBox.Initialized += (sender, args) => MessageTextBox.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}