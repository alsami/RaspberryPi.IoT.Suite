using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace RaspberryPi.IoT.Suite.Services
{
    public class DeviceClientAdapter : IDisposable
    {
        private ConnectionStatus connectionStatus = ConnectionStatus.Disconnected;

        private readonly SemaphoreSlim mutex = new(1);
        private readonly DeviceClient deviceClient;
        

        public DeviceClientAdapter(DeviceClient deviceClient)
        {
            this.deviceClient = deviceClient;
            this.deviceClient.SetConnectionStatusChangesHandler(this.StatusChangesHandler);
        }
        
        public void Dispose()
        {
            this.deviceClient.SetConnectionStatusChangesHandler(null);
            this.mutex.Dispose();
            GC.SuppressFinalize(this);
        }

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        public DeviceClient DeviceClient => this.deviceClient;

        public async Task OpenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.mutex.WaitAsync(cancellationToken);
                if (this.connectionStatus == ConnectionStatus.Connected)
                {
                    return;
                }

                await this.deviceClient.OpenAsync(cancellationToken);
            }
            finally
            {
                this.mutex.Release();
            }
        }

        private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            this.connectionStatus = status;
        }
    }
}