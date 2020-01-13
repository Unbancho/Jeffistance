using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace Jeffistance.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Casca gae";
        public string TheTruth => "Jeff ungae";
    
        public void OnClicked()
        {
            Console.WriteLine("casca gae");
        }
    }
}
