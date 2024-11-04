using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Avalonia;
using Avalonia.Android;
using BlueLinker.Core.Bluetooth;
using BlueLinker.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toast = Android.Widget.Toast;

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
        private const int RequestBluetoothPermissions = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckBluetoothPermissions();
            var serviceProvider = App.GetServiceProvider();
            Task.Run(() => (serviceProvider.GetService<IBluetoothService>() as AndroidBluetoothService)?.StartListeningForConnectionsAsync());
        }

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

        private void CheckBluetoothPermissions()
        {
            // Проверка необходимых разрешений
            var permissionsNeeded = new List<string>
            {
                Manifest.Permission.Bluetooth,
                Manifest.Permission.BluetoothAdmin,
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.AccessCoarseLocation
            };

            // Для Android 12 и выше нужно добавлять дополнительные разрешения
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                permissionsNeeded.Add(Manifest.Permission.BluetoothScan);
                permissionsNeeded.Add(Manifest.Permission.BluetoothConnect);
                permissionsNeeded.Add(Manifest.Permission.BluetoothAdvertise); // Опционально
            }

            // Проверка разрешений
            var permissionsToRequest = permissionsNeeded
                .Where(permission => ContextCompat.CheckSelfPermission(this, permission) != (int)Permission.Granted)
                .ToList();

            if (permissionsToRequest.Any())
            {
                // Запрос разрешений
                ActivityCompat.RequestPermissions(this, permissionsToRequest.ToArray(), RequestBluetoothPermissions);
            }
            else
            {
                // Все разрешения предоставлены, можно продолжать работу с Bluetooth
                OnBluetoothPermissionsGranted();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestBluetoothPermissions)
            {
                if (grantResults.Length > 0 && grantResults.All(result => result == Permission.Granted))
                {
                    // Разрешения получены, продолжаем работу с Bluetooth
                    OnBluetoothPermissionsGranted();
                }
                else
                {
                    // Разрешения не были получены, информируем пользователя
                    Toast.MakeText(this, "Для работы с Bluetooth необходимы разрешения.", ToastLength.Long).Show();
                }
            }
        }

        private void OnBluetoothPermissionsGranted()
        {
            // Здесь можно добавить логику для продолжения работы с Bluetooth
            Toast.MakeText(this, "Разрешения на использование Bluetooth получены.", ToastLength.Short).Show();
        }
    }
}