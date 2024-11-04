using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Avalonia;
using BlueLinker.Core.Bluetooth;
using Microsoft.Extensions.DependencyInjection;

namespace BlueLinker.Android
{
    [Activity(Label = "BlueLinker.Android", Theme = "@style/MyTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : Avalonia.Android.AvaloniaMainActivity<App>
    {
        private const int RequestBluetoothPermissions = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckBluetoothPermissions();
        }

        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            var serviceCollection = new ServiceCollection();
            RegisterAndroidServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            App.SetServiceProvider(serviceProvider);
            return base.CustomizeAppBuilder(builder)
                .WithInterFont(); // Используем сервис-провайдер
        }

        private void RegisterAndroidServices(IServiceCollection services)
        {
            services.AddSingleton<IBluetoothService, AndroidBluetoothService>();
            // Регистрация других необходимых сервисов
        }

        private void CheckBluetoothPermissions()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdmin) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothScan) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[]
                {
                    Manifest.Permission.Bluetooth,
                    Manifest.Permission.BluetoothAdmin,
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect
                }, RequestBluetoothPermissions);
            }
            else
            {
                Toast.MakeText(this, "Bluetooth разрешения получены.", ToastLength.Short).Show();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestBluetoothPermissions)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    Toast.MakeText(this, "Разрешения получены.", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Для работы приложения требуются разрешения на Bluetooth.", ToastLength.Long).Show();
                }
            }
        }
    }
}