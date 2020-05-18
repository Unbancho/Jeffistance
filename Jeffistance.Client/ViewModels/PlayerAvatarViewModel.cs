using Avalonia.Controls;
using ReactiveUI;
using Avalonia.Media.Imaging;

namespace Jeffistance.Client.ViewModels
{
    public class PlayerAvatarViewModel : Control
    {
        public const int ImageWidth = 50;
        public const int ImageHeight = 50;
        
        public PlayerAvatarViewModel(string username)
        {
            Image = new Image();
            Image.Source = new Bitmap("Jeffistance.Client\\Assets\\Spy.png");
            Image.Width = ImageWidth;
            Image.Height = ImageHeight;
            Username = username;
        }

        Image Image;
        string Username;

    }
}