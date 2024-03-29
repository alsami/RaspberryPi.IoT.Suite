namespace RaspberryPi.IoT.Suite.Services.Abstractions;

public interface IDeployMemoryQueueAdapter<TDeploymentOption> where TDeploymentOption : class, IDeploymentOption
{
    ValueTask Write(TDeploymentOption deploymentOption,
        CancellationToken cancellationToken = default);
    ValueTask SubscribeAsync(Func<TDeploymentOption, Task> callBack, CancellationToken cancellationToken = default);
}