using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Devices.Client;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands;

namespace RaspberryPi.IoT.Suite.UseCases.Commands
{
    public class TransmitHeartbeatCommandHandler : IRequestHandler<TransmitHeartbeatCommand>
    {
        private const string HeartbeatKey = "heartbeat";
        
        private readonly DeviceClient deviceClient;

        public TransmitHeartbeatCommandHandler(DeviceClient deviceClient)
        {
            this.deviceClient = deviceClient;
        }

        public async Task<Unit> Handle(TransmitHeartbeatCommand request, CancellationToken cancellationToken)
        {
            var message = new Message(Array.Empty<byte>());
            message.Properties.Add(HeartbeatKey, request.FrequencyInSeconds.ToString(CultureInfo.InvariantCulture));
            await this.deviceClient.OpenAsync(cancellationToken);
            await this.deviceClient.SendEventAsync(message, cancellationToken);
            await this.deviceClient.CloseAsync(cancellationToken);
            return Unit.Value;
        }
    }
}