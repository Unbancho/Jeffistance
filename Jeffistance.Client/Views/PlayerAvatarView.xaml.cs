using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Jeffistance.Client.ViewModels;
using Avalonia.Input;
using System;

namespace Jeffistance.Client.Views
{
    public class PlayerAvatarView : UserControl
    {
        private readonly Image avatar;
        private readonly TextBox username;

        private bool Selected;

        public PlayerAvatarView()
        {
        }
        public PlayerAvatarView(string playerName)
        {
            InitializeComponent();
            avatar = this.FindControl<Image>("avatar");
            username = this.FindControl<TextBox>("username");
            username.Text = playerName;
            avatar.PointerPressed += onImageClicked;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void onImageClicked(object sender, PointerPressedEventArgs args)
        {
           Selected = !Selected;
        }
    }
}