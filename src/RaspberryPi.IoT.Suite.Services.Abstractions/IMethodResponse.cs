using System.Net;

namespace RaspberryPi.IoT.Suite.Services.Abstractions
{
    public interface IMethodResponse
    {
        HttpStatusCode StatusCode { get; }
    }
}