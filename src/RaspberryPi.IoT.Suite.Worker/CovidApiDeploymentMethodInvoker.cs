using System.Net;
using Microsoft.Extensions.Hosting;
using RaspberryPi.IoT.Suite.Services.Abstractions;

namespace RaspberryPi.IoT.Suite.Worker;

public class CovidApiDeploymentMethodInvoker : BackgroundService
{
    private readonly IDeployMemoryQueueAdapter<CovidStatisticsApiDeploymentOption> deployMemoryQueueAdapter;
    private readonly IMethodHandler<CovidStatisticsApiDeploymentOption, MessageMethodResponse> methodHandler;

    public CovidApiDeploymentMethodInvoker(IDeployMemoryQueueAdapter<CovidStatisticsApiDeploymentOption> deployMemoryQueueAdapter, IMethodHandler<CovidStatisticsApiDeploymentOption, MessageMethodResponse> methodHandler)
    {
        this.deployMemoryQueueAdapter = deployMemoryQueueAdapter;
        this.methodHandler = methodHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.methodHandler.RegisterHandlerAsync(DeviceMethod.CovidStatisticsApiDeployment, 
            options => this.DefaultMethodResponse(options, stoppingToken),
            stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task<MessageMethodResponse> DefaultMethodResponse(CovidStatisticsApiDeploymentOption? options, CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(options?.Tag))
        {
            return new MessageMethodResponse("Tag must be given!", HttpStatusCode.BadRequest);
        }
            
        await this.deployMemoryQueueAdapter.Write(options, stoppingToken);
        return new MessageMethodResponse("Deployment queued!", HttpStatusCode.Accepted);
    }
}