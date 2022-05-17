using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands;

namespace RaspberryPi.IoT.Suite.Worker;

public class HeartbeatSenderWorker : BackgroundService
{
    private readonly ILogger<HeartbeatSenderWorker> logger;
    private readonly IServiceProvider serviceProvider;

    public HeartbeatSenderWorker(ILogger<HeartbeatSenderWorker> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    protected override async  Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var next = DateTime.UtcNow;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.UtcNow >= next)
            {
                await this.TransmitHeartbeatAsync();
                next = DateTime.UtcNow.AddMinutes(10);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task TransmitHeartbeatAsync()
    {
        this.logger.LogInformation("Transmitting heartbeat");
        using var scope = this.serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var command = new TransmitHeartbeatCommand(60 * 60);
        await mediator.Send(command);
        this.logger.LogInformation("Heartbeat transmitted");
    }
}