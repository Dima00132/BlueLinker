using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueLinker.Core.Bluetooth;
using Java.Util;

namespace BlueLinker.Android
{
    public class AndroidBluetoothService : IBluetoothService
    {
        
        private BluetoothSocket _socket;

        private BluetoothAdapter _adapter;
        private BluetoothServerSocket _serverSocket;

        private const string UUID_STRING = "00001101-0000-1000-8000-00805F9B34FB";

        public AndroidBluetoothService()
        {
            _adapter = BluetoothAdapter.DefaultAdapter;
        }

        public async Task StartListeningForConnectionsAsync()
        {
            UUID uuid = UUID.FromString(UUID_STRING);
            _serverSocket = _adapter.ListenUsingRfcommWithServiceRecord("MyBluetoothService", uuid);

            while (true)
            {
                try
                {
                    BluetoothSocket socket = await Task.Run(() => _serverSocket.Accept());
                    if (socket != null)
                    {
                        // Обработка нового подключения
                        OnDeviceConnected(socket);
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при ожидании подключения: {ex.Message}");
                }
            }
        }

        private async void OnDeviceConnected(BluetoothSocket socket)
        {
            // Логика обработки подключения
            try
            {
                // Получаем поток для чтения и записи данных
                using var stream = socket.InputStream;
                using var outputStream = socket.OutputStream;

                // Пример: Отправка приветственного сообщения
                var welcomeMessage = Encoding.UTF8.GetBytes("Привет от Android!");
                await outputStream.WriteAsync(welcomeMessage, 0, welcomeMessage.Length);

                // Чтение данных от подключенного устройства
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено: {receivedData}");

                    // Эхо-ответ на полученные данные
                    await outputStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке подключения: {ex.Message}");
            }
            finally
            {
                socket.Close();
            }
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