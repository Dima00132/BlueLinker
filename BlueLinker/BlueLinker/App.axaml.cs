using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BlueLinker.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlueLinker
{
    public class App : Application
    {
        private static IServiceProvider _serviceProvider;

        public App()
        {
            // Пустой конструктор для соответствия требованиям
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            // Здесь регистрируйте ваши общие сервисы
        }

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new WindowsView
                {
                    DataContext = _serviceProvider.GetService<WindowsViewModel>()
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new AndroidView
                {
                    DataContext = _serviceProvider.GetService<AndroidViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}