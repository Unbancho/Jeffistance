using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Jeffistance.Client.ViewModels;
using Jeffistance.Client.Views;
using Jeffistance.Common.Services.IoC;
using Jeffistance.Common.Services;

using System;

namespace Jeffistance.Client
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            RegisterClientDependencies();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        public void RegisterClientDependencies()
        {
            IoCManager.Register<IClientMessageFactory, ClientMessageFactory>();

            IoCManager.BuildGraph();
        }
    }
}