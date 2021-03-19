using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RaspberryPi.IoT.Suite.Services.Abstractions;

namespace RaspberryPi.IoT.Suite.Services
{
    public class MethodHandler<TRequest, TResponse> : IMethodHandler<TRequest, TResponse> where TRequest : class where TResponse : class, IMethodResponse
    {
        private readonly DeviceClientAdapter deviceClientAdapter;

        private readonly JsonSerializerSettings jsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private Func<TRequest?, Task<TResponse>>? internalMethodAccessor;

        public MethodHandler(DeviceClientAdapter deviceClientAdapter)
        {
            this.deviceClientAdapter = deviceClientAdapter;
        }

        public async Task RegisterHandlerAsync(DeviceMethod method, Func<TRequest?, Task<TResponse>> methodAccessor, CancellationToken cancellationToken = default)
        {
            this.internalMethodAccessor = methodAccessor ?? throw new ArgumentNullException(nameof(methodAccessor));
            await this.deviceClientAdapter.OpenAsync(cancellationToken);
            await this.deviceClientAdapter.DeviceClient.SetMethodHandlerAsync(method.GetMethodNameFor(), this.DeviceMethodHandler, new object(), cancellationToken);
        }

        private async Task<MethodResponse> DeviceMethodHandler(MethodRequest methodRequest, object userContext)
        {
            if (this.internalMethodAccessor is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(this.internalMethodAccessor)} is null and needs to be registered for this operation!");
            }

            var request = JsonConvert.DeserializeObject<TRequest>(methodRequest.DataAsJson, this.jsonSerializerSettings);
            var response = await this.internalMethodAccessor!.Invoke(request);
            return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response, this.jsonSerializerSettings)), (int) response.StatusCode);
        }
    }
}