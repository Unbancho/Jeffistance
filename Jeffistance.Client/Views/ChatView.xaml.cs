using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Client.Views
{
    public class ChatView : UserControl
    {
        public TextBox MessageTextBox => this.FindControl<TextBox>("MessageContentBox");

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