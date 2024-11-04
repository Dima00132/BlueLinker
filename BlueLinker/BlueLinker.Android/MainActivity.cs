using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls.ApplicationLifetimes;
using BlueLinker.Core.Bluetooth;
using BlueLinker.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BlueLinker.Android
{
    [Activity(
        Label = "BlueLinker.Android",
        Theme = "@style/MyTheme.NoActionBar",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class MainActivity : AvaloniaMainActivity<App>
    {
        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            var serviceCollection = new ServiceCollection();

            // Регистрация сервисов для Android
            RegisterAndroidServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            App.SetServiceProvider(serviceProvider); // Устанавливаем сервис-провайдер

            return base.CustomizeAppBuilder(builder)
                .WithInterFont();
        }

        private void RegisterAndroidServices(IServiceCollection services)
        {
            services.AddSingleton<IBluetoothService, AndroidBluetoothService>();
            services.AddTransient<AndroidViewModel>();
            // Добавьте другие зависимости Android, если необходимо
        }
    }
}
