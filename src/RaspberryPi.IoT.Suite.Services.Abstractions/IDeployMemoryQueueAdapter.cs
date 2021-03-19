using System;
using System.Threading;
using System.Threading.Tasks;

namespace RaspberryPi.IoT.Suite.Services.Abstractions
{
    public interface IDeployMemoryQueueAdapter<TDeploymentOption> where TDeploymentOption : class, IDeploymentOption
    {
        ValueTask Write(TDeploymentOption deploymentOption,
            CancellationToken cancellationToken = default);
        ValueTask Subscribe(Func<TDeploymentOption, Task> callBack);
    }
}