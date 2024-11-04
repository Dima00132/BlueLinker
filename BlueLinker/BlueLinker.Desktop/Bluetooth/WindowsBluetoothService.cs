using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net.Sockets; // Предполагается, что библиотека для Bluetooth уже подключена
using BlueLinker.Core.Bluetooth;
using InTheHand.Net.Bluetooth; // Пространство имен для IBluetoothService

namespace BlueLinker.Desktop
{
    /// <summary>
    /// Реализация интерфейса <see cref="IBluetoothService"/> для работы с Bluetooth на Windows.
    /// </summary>
    public class WindowsBluetoothService : IBluetoothService
    {
        private BluetoothClient _client;

        /// <summary>
        /// Подключается к указанному Bluetooth-устройству по имени или адресу.
        /// </summary>
        /// <param name="deviceNameOrAddress">Имя или адрес Bluetooth-устройства.</param>
        /// <returns>Возвращает <c>true</c>, если подключение выполнено успешно.</returns>
        /// <exception cref="Exception">Выбрасывается, если устройство не найдено.</exception>
        public async Task<bool> ConnectAsync(string deviceNameOrAddress)
        {
            _client = new BluetoothClient();
            var devices = _client.DiscoverDevices();
            var device = devices.FirstOrDefault(d => d.DeviceName == deviceNameOrAddress || d.DeviceAddress.ToString() == deviceNameOrAddress);

            if (device == null)
                throw new Exception("Устройство не найдено.");

            await Task.Run(() => _client.Connect(device.DeviceAddress, BluetoothService.SerialPort));
            return _client.Connected;
        }

        /// <summary>
        /// Отправляет данные на подключенное Bluetooth-устройство.
        /// </summary>
        /// <param name="data">Строка данных для отправки.</param>
        /// <exception cref="InvalidOperationException">Выбрасывается, если соединение не установлено.</exception>
        public async Task SendDataAsync(string data)
        {
            if (_client == null || !_client.Connected)
                throw new InvalidOperationException("Соединение не установлено.");

            var dataBytes = Encoding.UTF8.GetBytes(data); // Сериализация строки
            using var stream = _client.GetStream();
            await stream.WriteAsync(dataBytes, 0, dataBytes.Length);
        }

        /// <summary>
        /// Получает данные с подключенного Bluetooth-устройства.
        /// </summary>
        /// <returns>Возвращает полученные данные в виде строки.</returns>
        /// <exception cref="InvalidOperationException">Выбрасывается, если соединение не установлено.</exception>
        public async Task<string> ReceiveDataAsync()
        {
            if (_client == null || !_client.Connected)
                throw new InvalidOperationException("Соединение не установлено.");

            using var stream = _client.GetStream();
            using var memoryStream = new MemoryStream();
            var buffer = new byte[256];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray()); // Десериализация строки
        }

        /// <summary>
        /// Отключает текущее соединение с Bluetooth-устройством.
        /// </summary>
        public void Disconnect()
        {
            _client?.Close();
            _client = null;
        }
    }
}