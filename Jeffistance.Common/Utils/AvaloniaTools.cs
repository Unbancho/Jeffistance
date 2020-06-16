using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Jeffistance.Common.AvaloniaTools
{
    public static class AvaloniaTools
    {
        public static Bitmap GetImageFromResources(string assamble, string imageName)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            Uri uri = new Uri("avares://" + assamble + "/Assets/" + imageName);
            return new Bitmap(assets.Open(uri));
        }
    }
}