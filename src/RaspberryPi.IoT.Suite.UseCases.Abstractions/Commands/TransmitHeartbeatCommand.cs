using MediatR;

namespace RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands
{
    public record TransmitHeartbeatCommand(uint FrequencyInSeconds) : IRequest;
}