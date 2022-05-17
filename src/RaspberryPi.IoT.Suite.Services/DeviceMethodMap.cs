using RaspberryPi.IoT.Suite.Services.Abstractions;

namespace RaspberryPi.IoT.Suite.Services;

internal static class DeviceMethodMap
{
    private static readonly IReadOnlyDictionary<DeviceMethod, string> DeviceMethodNameByDeviceMethod =
        new Dictionary<DeviceMethod, string>
        {
            [DeviceMethod.CovidStatisticsApiDeployment] = "covid-statistics-api-deployment",
            [DeviceMethod.CovidStatisticsAppDeployment] = "covid-statistics-app-deployment",
            [DeviceMethod.ClockMeasurement] = "clock-measurement"
        };
        
    public static string GetMethodNameFor(this DeviceMethod method)
    {
        return DeviceMethodNameByDeviceMethod.ContainsKey(method)
            ? DeviceMethodNameByDeviceMethod[method]
            : throw new ArgumentException($"No method mapped for given {nameof(DeviceMethod)} with value {method.ToString()}", nameof(method));
    }
}