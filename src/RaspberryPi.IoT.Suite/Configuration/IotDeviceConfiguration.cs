// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace RaspberryPi.IoT.Suite.Configuration;

public class IotDeviceConfiguration
{
    public string Host { get; set; } = null!;

    public string DeviceId { get; set; } = null!;

    public string DeviceKey { get; set; } = null!;
}