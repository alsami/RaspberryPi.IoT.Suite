using System;
using System.Threading;
using System.Threading.Tasks;

namespace RaspberryPi.IoT.Suite.Services.Abstractions
{
    public interface IMethodHandler<out TRequest, TResponse> where TRequest : class where TResponse : class, IMethodResponse
    {
        Task RegisterHandlerAsync(DeviceMethod method, Func<TRequest?, Task<TResponse>> methodAccessor, CancellationToken cancellationToken = default);
    }
}