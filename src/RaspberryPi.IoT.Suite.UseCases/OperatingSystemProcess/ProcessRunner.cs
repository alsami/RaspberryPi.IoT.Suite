using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RaspberryPi.IoT.Suite.Exceptions;

namespace RaspberryPi.IoT.Suite.UseCases.OperatingSystemProcess
{
    public static class ProcessRunner
    {
        public static async Task<TResult> RunAsync<TResult>(ProcessStartInfo processStartInfo,
            Func<string, TResult> resultConverter)
        {
            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                throw new ProcessExecutionFailureException("Process could not be started!", -1);
            }

            var output = await process.StandardOutput.ReadToEndAsync();

            process.WaitForExit(3000);

            if (process.ExitCode != 0)
            {
                throw new ProcessExecutionFailureException(await process.StandardError.ReadToEndAsync(), process.ExitCode);
            }
            
            return resultConverter.Invoke(output);
        }
    }
}