using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
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

        private const int RequestBluetoothPermissions = 1;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Проверка и запрос разрешений для работы с Bluetooth

            CheckBluetoothPermissions();
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
            // Проверка разрешений
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdmin) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothScan) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothConnect) != (int)Permission.Granted ||
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothAdvertise) != (int)Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, Manifest.Permission.BluetoothPrivileged) != (int)Permission.Granted )
            {
                // Запрос разрешений
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.Bluetooth,
                    Manifest.Permission.BluetoothAdmin,
                    Manifest.Permission.BluetoothAdvertise,
                    Manifest.Permission.BluetoothPrivileged,
                    Manifest.Permission.BluetoothScan,  // Для Android 12 и выше
                    Manifest.Permission.BluetoothConnect // Для Android 12 и выше
                }, RequestBluetoothPermissions);
            }
            else
            {
                // Разрешения уже предоставлены, продолжайте работу с Bluetooth
                OnBluetoothPermissionsGranted();
            }
        }

        private void ShowBluetoothNotSupportedMessage()
        {
            // Обработка случая, когда Bluetooth не поддерживается
            Toast.MakeText(this, "Bluetooth не поддерживается на этом устройстве.", ToastLength.Long).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestBluetoothPermissions)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // Разрешение получено, продолжаем операции с Bluetooth
                    OnBluetoothPermissionsGranted();
                }
                else
                {
                    // Разрешение отказано, показываем сообщение пользователю
                    Toast.MakeText(this, "Для этого приложения требуется разрешение на использование Bluetooth.", ToastLength.Long).Show();
                }
            }
        }

        private void OnBluetoothPermissionsGranted()
        {
            // Логика для продолжения работы с Bluetooth после получения разрешений
            Toast.MakeText(this, "Разрешения на использование Bluetooth получены.", ToastLength.Short).Show();
            // Здесь можно добавить дополнительную логику инициализации Bluetooth
        }
    }
}
