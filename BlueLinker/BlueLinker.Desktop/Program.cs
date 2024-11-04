using Avalonia;
using BlueLinker.Core.Bluetooth;
using BlueLinker.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlueLinker.Desktop
{
    internal sealed class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
        {
            var serviceCollection = new ServiceCollection();
            RegisterWindowsServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            App.SetServiceProvider(serviceProvider); // Устанавливаем сервис-провайдер

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont(); // Используем сервис-провайдер
        }

        private static void RegisterWindowsServices(IServiceCollection services)
        {
            services.AddSingleton<IBluetoothService, WindowsBluetoothService>();
            services.AddTransient<WindowsViewModel>();
            // Добавьте другие зависимости Windows, если необходимо
        }
    }
}
