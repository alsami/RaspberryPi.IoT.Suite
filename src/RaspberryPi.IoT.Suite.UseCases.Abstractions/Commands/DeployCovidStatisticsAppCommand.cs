using MediatR;

namespace RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands
{
    public record DeployCovidStatisticsAppCommand(string Tag) : IRequest;
}