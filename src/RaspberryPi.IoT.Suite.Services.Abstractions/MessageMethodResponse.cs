using System.Net;

namespace RaspberryPi.IoT.Suite.Services.Abstractions
{
    public record MessageMethodResponse(string Message, HttpStatusCode StatusCode) : IMethodResponse;
}