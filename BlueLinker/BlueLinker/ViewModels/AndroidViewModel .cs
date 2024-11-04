using BlueLinker.Core.Bluetooth;
using BlueLinker.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BlueLinker.ViewModels
{
    public partial class AndroidViewModel : ViewModelBase
    {
        private readonly IBluetoothService _bluetoothService;
        private string _status;

        public AndroidViewModel(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        [RelayCommand]
        public async Task ConnectAsync()
        {
            Status = "Подключение...";
            try
            {
                bool isConnected = await _bluetoothService.ConnectAsync("MSI");
                Status = isConnected ? "Подключено" : "Не удалось подключиться";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка подключения: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task SendDataAsync()
        {
            try
            {
                await _bluetoothService.SendDataAsync("Hello from Android!");
                Status = "Данные отправлены";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка отправки данных: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task ReceiveDataAsync()
        {
            try
            {
                var data = await _bluetoothService.ReceiveDataAsync();
                Status = $"Получено: {data}";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка получения данных: {ex.Message}";
            }
        }
    }
}