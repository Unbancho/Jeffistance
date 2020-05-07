using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Jeffistance.Views
{
    public class ChatMessageView : UserControl
    {
        public TextBox UsernameTextBox => this.FindControl<TextBox>("MessageContentBox");
        public Button EditButton => this.FindControl<Button>("EditButton");
        public ChatMessageView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}