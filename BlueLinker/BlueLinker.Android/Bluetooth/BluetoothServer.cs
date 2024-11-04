using Android.Bluetooth;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlueLinker.Android.Bluetooth
{
    public class BluetoothServer
    {
        private BluetoothServerSocket _serverSocket;
        private const string UUID_STRING = "00001101-0000-1000-8000-00805f9b34fb"; // UUID для Serial Port Profile

        public async Task StartServerAsync()
        {
            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
            {
                Console.WriteLine("Bluetooth не поддерживается или отключен.");
                return;
            }

            try
            {
                // Создание серверного сокета
                _serverSocket = bluetoothAdapter.ListenUsingRfcommWithServiceRecord("MyBluetoothService", Java.Util.UUID.FromString(UUID_STRING));

                Console.WriteLine("Сервер Bluetooth запущен, ожидание подключения...");

                while (true)
                {
                    // Ожидание входящего подключения
                    var socket = await _serverSocket.AcceptAsync();
                    if (socket != null)
                    {
                        Console.WriteLine("Подключение установлено с: " + socket.RemoteDevice.Name);
                        // Здесь можно начать взаимодействие с клиентом
                        await HandleClientSocket(socket);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске сервера: {ex.Message}");
            }
        }

        private async Task HandleClientSocket(BluetoothSocket socket)
        {
            using (var inputStream = socket.InputStream)
            using (var outputStream = socket.OutputStream)
            {
                // Чтение данных от клиента
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var receivedData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получены данные: {receivedData}");

                    // Пример: отправка ответа обратно
                    var response = "Сообщение получено";
                    var responseBytes = System.Text.Encoding.UTF8.GetBytes(response);
                    await outputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }

            // Закрытие сокета после завершения работы с клиентом
            socket.Close();
            Console.WriteLine("Клиент отключился.");
        }
    }
}
