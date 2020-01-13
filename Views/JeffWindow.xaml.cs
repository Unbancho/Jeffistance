using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Input;

using System;

namespace Jeffistance.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private TimeSpan _firstTime;
        public TimeSpan ElapsedTime{
            get{ return DateTime.Now.TimeOfDay - _firstTime; }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            int elapsedTime = (int) ElapsedTime.TotalSeconds;
            ((Button) sender).Content = String.Format("Factorial of {0} = {1}", elapsedTime, Factorial((ulong)elapsedTime));
        }

        private void OnPointerEnter(object sender, PointerEventArgs e)
        {
            _firstTime = DateTime.Now.TimeOfDay;
        }

        private ulong Factorial(ulong n)
        {
            return n == 0 ? 1 : n*Factorial(n-1);
        }

        private void OnPointerLeave(object sender, PointerEventArgs e)
        {
        }
    }
}