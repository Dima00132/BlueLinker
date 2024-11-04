
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLinker.Core.Bluetooth
{
    /// <summary>
    /// Интерфейс для работы с Bluetooth-сервисами.
    /// </summary>
    public interface IBluetoothService
    {
        /// <summary>
        /// Подключается к указанному Bluetooth-устройству по имени или адресу.
        /// </summary>
        /// <param name="deviceNameOrAddress">Имя или адрес Bluetooth-устройства.</param>
        /// <returns>Возвращает <c>true</c>, если подключение выполнено успешно.</returns>
        Task<bool> ConnectAsync(string deviceNameOrAddress);

        /// <summary>
        /// Отправляет данные на подключенное Bluetooth-устройство.
        /// </summary>
        /// <param name="data">Строка данных для отправки.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        Task SendDataAsync(string data);

        /// <summary>
        /// Получает данные с подключенного Bluetooth-устройства.
        /// </summary>
        /// <returns>Возвращает полученные данные в виде строки.</returns>
        Task<string> ReceiveDataAsync();

        /// <summary>
        /// Отключает текущее соединение с Bluetooth-устройством.
        /// </summary>
        void Disconnect();
    }
}
