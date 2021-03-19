using System.Net;

namespace RaspberryPi.IoT.Suite.Services.Abstractions
{
    public record ObjectMethodResponse<T>(T? Content, HttpStatusCode StatusCode) : IMethodResponse where T: class;
}