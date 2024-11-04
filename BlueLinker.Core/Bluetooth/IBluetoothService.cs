using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLinker.Core.Bluetooth
{
    public interface IBluetoothService
    {
        Task<bool> ConnectAsync(string deviceNameOrAddress);
        Task SendDataAsync(string data);
        Task<string> ReceiveDataAsync();
        void Disconnect();
    }
}
