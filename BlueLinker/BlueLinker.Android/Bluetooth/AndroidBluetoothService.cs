using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueLinker.Core.Bluetooth;

namespace BlueLinker.Android
{
    public class AndroidBluetoothService : IBluetoothService
    {
        private readonly BluetoothAdapter _adapter;
        private BluetoothSocket _socket;

        private static readonly string UUID_STRING = "00001101-0000-1000-8000-00805f9b34fb"; // UUID для Serial Port

        public AndroidBluetoothService()
        {
            _adapter = BluetoothAdapter.DefaultAdapter;
        }

        public async Task<bool> ConnectAsync(string deviceNameOrAddress)
        {
            if (_adapter == null || !_adapter.IsEnabled)
                throw new Exception("Bluetooth не доступен или не включен.");

            // Убедитесь, что устройство находится в режиме видимости
            EnableBluetoothVisibility();

            var device = _adapter.BondedDevices.FirstOrDefault(d => d.Name == deviceNameOrAddress || d.Address == deviceNameOrAddress);
            if (device == null)
                throw new Exception("Устройство не найдено среди сопряженных.");

            var serviceUUID = Java.Util.UUID.FromString(UUID_STRING);

            try
            {
                _socket = device.CreateRfcommSocketToServiceRecord(serviceUUID);
                await _socket.ConnectAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка подключения: {ex.Message}");
            }

            return _socket.IsConnected;
        }

        public async Task SendDataAsync(string data)
        {
            if (_socket == null || !_socket.IsConnected)
                throw new InvalidOperationException("Соединение не установлено.");

            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var stream = _socket.OutputStream;
            await stream.WriteAsync(dataBytes, 0, dataBytes.Length);
        }

        public async Task<string> ReceiveDataAsync()
        {
            if (_socket == null || !_socket.IsConnected)
                throw new InvalidOperationException("Соединение не установлено.");

            using var stream = _socket.InputStream;
            var buffer = new byte[256];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void Disconnect()
        {
            _socket?.Close();
            _socket = null;
        }

        private void EnableBluetoothVisibility()
        {
            // Логика для установки устройства в режим видимости
        }
    }
}