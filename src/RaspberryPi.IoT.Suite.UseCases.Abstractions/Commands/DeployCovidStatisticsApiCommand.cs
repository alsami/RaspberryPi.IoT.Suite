using MediatR;

namespace RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands;

public record DeployCovidStatisticsApiCommand(string Tag) : IRequest;