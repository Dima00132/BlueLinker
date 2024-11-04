using BlueLinker.Core.Bluetooth;
using BlueLinker.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BlueLinker.ViewModels
{
    public partial class WindowsViewModel : ViewModelBase
    {
        private readonly IBluetoothService _bluetoothService;
        private string _status;

        public WindowsViewModel(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        [RelayCommand]
        private async Task ConnectAsync()
        {
            Status = "Подключение...";
            try
            {
                bool isConnected = await _bluetoothService.ConnectAsync("DeviceNameOrAddress");
                Status = isConnected ? "Подключено" : "Не удалось подключиться";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка подключения: {GetErrorMessage(ex)}";
            }
        }

        [RelayCommand]
        private async Task SendDataAsync()
        {
            try
            {
                await _bluetoothService.SendDataAsync("Hello from Windows!");
                Status = "Данные отправлены";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка отправки данных: {GetErrorMessage(ex)}";
            }
        }

        [RelayCommand]
        private async Task ReceiveDataAsync()
        {
            try
            {
                var data = await _bluetoothService.ReceiveDataAsync();
                Status = $"Получено: {data}";
            }
            catch (Exception ex)
            {
                Status = $"Ошибка получения данных: {GetErrorMessage(ex)}";
            }
        }

        private string GetErrorMessage(Exception ex)
        {
            return ex.Message;
        }
    }
}