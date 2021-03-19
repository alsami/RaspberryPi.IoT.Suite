using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands;
using RaspberryPi.IoT.Suite.UseCases.Configuration;
using RaspberryPi.IoT.Suite.UseCases.OperatingSystemProcess;

namespace RaspberryPi.IoT.Suite.UseCases.Commands
{
    public class DeployCovidStatisticsApiCommandHandler : IRequestHandler<DeployCovidStatisticsApiCommand>
    {
        private readonly ILogger<DeployCovidStatisticsApiCommandHandler> logger;
        private readonly IOptions<CovidStatisticsApiDeploymentConfiguration> deploymentOptions;

        public DeployCovidStatisticsApiCommandHandler(ILogger<DeployCovidStatisticsApiCommandHandler> logger, IOptions<CovidStatisticsApiDeploymentConfiguration> deploymentOptions)
        {
            this.logger = logger;
            this.deploymentOptions = deploymentOptions;
        }

        public async Task<Unit> Handle(DeployCovidStatisticsApiCommand request, CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Deploying Service Covid-Statistics API with Version {Version}", request.Tag);
            this.logger.LogInformation("Script configuration {ScriptConfiguration}", JsonSerializer.Serialize(this.deploymentOptions.Value));
                
            var args = new[] {this.deploymentOptions.Value.ScriptPath, request.Tag};
            var processStartInfo = ProcessStartInfoFactory.Create(Executables.Bash, args);
            await ProcessRunner.RunAsync(processStartInfo, _ => new object());
            
            this.logger.LogInformation("Deployed Service Covid-Statistics API with Version {Version}", request.Tag);
            return Unit.Value;
        }
    }
}