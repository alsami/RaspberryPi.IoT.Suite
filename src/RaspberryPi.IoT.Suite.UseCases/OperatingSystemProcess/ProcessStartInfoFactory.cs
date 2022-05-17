using System.Diagnostics;
using System.Text;

namespace RaspberryPi.IoT.Suite.UseCases.OperatingSystemProcess;

public static class ProcessStartInfoFactory
{
    public static ProcessStartInfo Create(string executableName, params string[] args)
    {
        return new(executableName, string.Join(" ", args))
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8
        };
    }
}