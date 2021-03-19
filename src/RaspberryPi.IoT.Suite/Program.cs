using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaspberryPi.IoT.Suite.Configuration;
using RaspberryPi.IoT.Suite.Services;
using RaspberryPi.IoT.Suite.Services.Abstractions;
using RaspberryPi.IoT.Suite.UseCases.Commands;
using RaspberryPi.IoT.Suite.UseCases.Configuration;
using RaspberryPi.IoT.Suite.Worker;
using Serilog;

namespace RaspberryPi.IoT.Suite
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = BuildHost(args);

            await host.RunAsync();
        }

        private static IHost BuildHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog(ConfigureLogger)
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer)
                .ConfigureServices(ConfigureServices)
                .Build();

        private static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        }

        private static void ConfigureContainer(HostBuilderContext hostBuilderContext, ContainerBuilder builder)
        {
            builder.Register(_ => ConfigureDeviceClient(hostBuilderContext))
                .As<DeviceClient>()
                .SingleInstance();
            
            builder.RegisterType<DeviceClientAdapter>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterGeneric(typeof(MethodHandler<,>))
                .As(typeof(IMethodHandler<,>))
                .InstancePerDependency();

            builder.RegisterType<CovidStatisticsDeployMemoryQueueAdapter>()
                .As<IDeployMemoryQueueAdapter<CovidStatisticsApiDeploymentOption>>()
                .SingleInstance();

            builder.RegisterMediatR(typeof(TransmitHeartbeatCommandHandler).Assembly);
        }

        private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.Configure<CovidStatisticsApiDeploymentConfiguration>(options =>
                hostBuilderContext.Configuration.Bind(nameof(CovidStatisticsApiDeploymentConfiguration), options));
            
            // services.AddHostedService<HeartbeatSenderWorker>();
            services.AddHostedService<CovidStatisticsApiDeploymentWorker>();
            services.AddHostedService<VcGenCommandMeasurementMethodInvoker>();
            services.AddHostedService<DeploymentMethodInvoker>();
        }
        
        private static DeviceClient ConfigureDeviceClient(HostBuilderContext hostBuilderContext)
        {
            var configuration = hostBuilderContext.Configuration.GetSection(nameof(IotDeviceConfiguration))
                .Get<IotDeviceConfiguration>();
            
            return DeviceClient.Create(configuration.Host,
                new DeviceAuthenticationWithRegistrySymmetricKey(configuration.DeviceId,
                    configuration.DeviceKey),
                TransportType.Mqtt);
        }
    }
}