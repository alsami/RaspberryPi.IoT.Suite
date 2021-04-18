using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MediatR;
using Microsoft.Extensions.Options;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Commands;
using RaspberryPi.IoT.Suite.UseCases.Configuration;
using RaspberryPi.IoT.Suite.UseCases.OperatingSystemProcess;

namespace RaspberryPi.IoT.Suite.UseCases.Commands
{
    public class DeployCovidStatisticsAppCommandHandler : IRequestHandler<DeployCovidStatisticsAppCommand>
    {
        private readonly BlobContainerClient blobContainerClient;
        private readonly IOptions<CovidStatisticsAppDeploymentConfiguration> covidStatisticsAppDeploymentConfigurationOptions;

        public DeployCovidStatisticsAppCommandHandler(BlobContainerClient blobContainerClient, IOptions<CovidStatisticsAppDeploymentConfiguration> covidStatisticsAppDeploymentConfigurationOptions)
        {
            this.blobContainerClient = blobContainerClient;
            this.covidStatisticsAppDeploymentConfigurationOptions = covidStatisticsAppDeploymentConfigurationOptions;
        }

        public async Task<Unit> Handle(DeployCovidStatisticsAppCommand request, CancellationToken cancellationToken)
        {
            var fileName = $"{request.Tag}.zip";
            var downloadDirectory = CreateDirectoryIfNotExists();
            var filePath = Path.Combine(downloadDirectory, fileName);
            DeleteFileIfExists(filePath);
            await this.DownloadAndStoreFileAsync(fileName, filePath, cancellationToken);
            var processInfo = ProcessStartInfoFactory.Create(
                Executables.Bash, 
                this.covidStatisticsAppDeploymentConfigurationOptions.Value.ScriptPath, 
                fileName, 
                filePath);
            await ProcessRunner.RunAsync(processInfo, _ => new object());
            return Unit.Value;
        }

        private static void DeleteFileIfExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }
            
            File.Delete(filePath);
        }

        private async ValueTask DownloadAndStoreFileAsync(string fileName, string filePath, CancellationToken cancellationToken)
        {
            var blobClient = this.blobContainerClient.GetBlobClient(fileName);
            var blobResponse = await blobClient.DownloadAsync(cancellationToken);

            await using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            await blobResponse.Value.Content.CopyToAsync(stream, cancellationToken);
        }

        private static string CreateDirectoryIfNotExists()
        {
            var downloadDirectory = Path.Combine(AppContext.BaseDirectory, "Downloads");

            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            return downloadDirectory;
        }
    }
}