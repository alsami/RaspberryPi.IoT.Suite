using System.Threading.Channels;
using RaspberryPi.IoT.Suite.Services.Abstractions;

namespace RaspberryPi.IoT.Suite.Services;

public abstract class GenericDeployMemoryQueueAdapter<TDeploymentOption> : IDeployMemoryQueueAdapter<TDeploymentOption> where TDeploymentOption : class, IDeploymentOption
{
    private readonly Channel<TDeploymentOption> channel = Channel.CreateUnbounded<TDeploymentOption>();

    public ValueTask Write(TDeploymentOption deploymentOption, CancellationToken cancellationToken = default) => this.channel.Writer.WriteAsync(deploymentOption, cancellationToken);

    public async ValueTask SubscribeAsync(Func<TDeploymentOption, Task> callBack, CancellationToken cancellationToken = default)
    {
        while (await this.channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (this.channel.Reader.TryRead(out var deploymentRequest))
            {
                await callBack.Invoke(deploymentRequest);
            }
        }
    }
}