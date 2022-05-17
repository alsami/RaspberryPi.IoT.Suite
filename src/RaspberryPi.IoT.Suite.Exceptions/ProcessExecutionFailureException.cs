namespace RaspberryPi.IoT.Suite.Exceptions;

public class ProcessExecutionFailureException : Exception
{
    public ProcessExecutionFailureException(string message, int processExitCode) : base(message)
    {
        this.ProcessExitCode = processExitCode;
    }
        
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public int ProcessExitCode { get; }
}