using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueLinker.Core.Bluetooth;
using System.IO;

namespace BlueLinker.Desktop
{
    public class WindowsBluetoothService : IBluetoothService
    {
        private BluetoothClient _client;
         private Stream _stream;
        private BluetoothListener _listener;
        private static readonly string UUID_STRING = "00001101-0000-1000-8000-00805f9b34fb"; // UUID для Serial Port


        public async Task StartListeningForConnectionsAsync()
        {
            Guid uuid = new Guid(UUID_STRING);
            _listener = new BluetoothListener(uuid);
            _listener.Start();

            while (true)
            {
                try
                {
                    var client = await Task.Run(() => _listener.AcceptBluetoothClient());
                    // Обработка нового подключения
                    OnDeviceConnected(client);
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    Console.WriteLine($"Ошибка при ожидании подключения: {ex.Message}");
                }
            }
        }

        private async void OnDeviceConnected(BluetoothClient client)
        {
            // Логика обработки подключения
            try
            {
                // Получаем поток для чтения и записи данных
                using var stream = client.GetStream();
                using var memoryStream = new MemoryStream();

                // Пример: Отправка приветственного сообщения
                var welcomeMessage = Encoding.UTF8.GetBytes("Привет от ПК!");
                await stream.WriteAsync(welcomeMessage, 0, welcomeMessage.Length);

                // Чтение данных от подключенного устройства
                var buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено: {receivedData}");

                    // Эхо-ответ на полученные данные
                    await stream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке подключения: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }


        public async Task<bool> ConnectAsync(string deviceNameOrAddress)
        {
            _client = new BluetoothClient();
            var devices = _client.DiscoverDevices();

            var device = devices.FirstOrDefault(d => d.DeviceName == deviceNameOrAddress || d.DeviceAddress.ToString() == deviceNameOrAddress);
            if (device == null)
                throw new Exception("Устройство не найдено.");

           

            try
            {
                await Task.Run(() => _client.Connect(device.DeviceAddress, new Guid( UUID_STRING)));
                _stream = _client.GetStream();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка подключения: {ex.Message}");
            }

            return _stream != null;
        }

        public async Task SendDataAsync(string data)
        {
            if (_stream == null)
                throw new InvalidOperationException("Соединение не установлено.");

            var dataBytes = Encoding.UTF8.GetBytes(data);
            await _stream.WriteAsync(dataBytes, 0, dataBytes.Length);
        }

        public async Task<string> ReceiveDataAsync()
        {
            if (_stream == null)
                throw new InvalidOperationException("Соединение не установлено.");

            var buffer = new byte[256];
            int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void Disconnect()
        {
            _client?.Close();
            _client = null;
        }
    }
}