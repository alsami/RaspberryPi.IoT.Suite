using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaspberryPi.IoT.Suite.Services.Abstractions;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands;

namespace RaspberryPi.IoT.Suite.Worker
{
    public class CovidStatisticsAppDeploymentWorker : BackgroundService
    {
        private readonly ILogger<CovidStatisticsAppDeploymentWorker> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IDeployMemoryQueueAdapter<CovidStatisticsAppDeploymentOption> deployMemoryQueueAdapter;

        public CovidStatisticsAppDeploymentWorker(
            ILogger<CovidStatisticsAppDeploymentWorker> logger, 
            IServiceProvider serviceProvider, 
            IDeployMemoryQueueAdapter<CovidStatisticsAppDeploymentOption> deployMemoryQueueAdapter)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.deployMemoryQueueAdapter = deployMemoryQueueAdapter;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this.deployMemoryQueueAdapter.SubscribeAsync(this.ProcessDeploymentAsync, stoppingToken);
        }

        private async Task ProcessDeploymentAsync(CovidStatisticsAppDeploymentOption deploymentOption)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new DeployCovidStatisticsAppCommand(deploymentOption.Tag));
            }
            catch (Exception e)
            {
                this.logger.LogCritical(e, "Failed to process {Request}", nameof(DeployCovidStatisticsAppCommand));
            }
        }
    }
}