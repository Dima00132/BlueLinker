using Android.Bluetooth;
using BlueLinker.Core.Bluetooth;
using Java.Util;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLinker.Core.Bluetooth
{
    public class AndroidBluetoothService : IBluetoothService
    {
        private BluetoothSocket _socket;

        public async Task<bool> ConnectAsync(string deviceNameOrAddress)
        {
            var adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null || !adapter.IsEnabled)
                throw new Exception("Bluetooth не доступен или не включен.");

            var device = adapter.BondedDevices.FirstOrDefault(d => d.Name == deviceNameOrAddress || d.Address == deviceNameOrAddress);
            if (device == null)
                throw new Exception("Устройство не найдено среди сопряженных.");

            UUID serviceUUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
            _socket = device.CreateRfcommSocketToServiceRecord(serviceUUID);

            await _socket.ConnectAsync();
            return _socket.IsConnected;
        }

        public async Task SendDataAsync(string data)
        {
            if (_socket == null || !_socket.IsConnected)
                throw new InvalidOperationException("Соединение не установлено.");

            var dataBytes = Encoding.UTF8.GetBytes(data); // Сериализация строки
            using var stream = _socket.OutputStream;
            await stream.WriteAsync(dataBytes, 0, dataBytes.Length);
        }

        public async Task<string> ReceiveDataAsync()
        {
            if (_socket == null || !_socket.IsConnected)
                throw new InvalidOperationException("Соединение не установлено.");

            using var stream = _socket.InputStream;
            using var memoryStream = new MemoryStream();
            var buffer = new byte[256];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, bytesRead);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray()); // Десериализация строки
        }

        public void Disconnect()
        {
            _socket?.Close();
            _socket = null;
        }
    }
}
