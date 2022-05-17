using System.Globalization;
using MediatR;
using Microsoft.Extensions.Logging;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Queries;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Response;
using RaspberryPi.IoT.Suite.UseCases.Extensions;
using RaspberryPi.IoT.Suite.UseCases.OperatingSystemProcess;
using UnitsNet;

namespace RaspberryPi.IoT.Suite.UseCases.Queries;

public class ReadClockMeasurementQueryHandler : IRequestHandler<ReadClockMeasurementQuery, ClockMeasureResponse>
{
    private readonly ILogger<ReadClockMeasurementQueryHandler> logger;

    public ReadClockMeasurementQueryHandler(ILogger<ReadClockMeasurementQueryHandler> logger)
    {
        this.logger = logger;
    }

    public async Task<ClockMeasureResponse> Handle(ReadClockMeasurementQuery request, CancellationToken cancellationToken)
    {
        var args = new[] {"measure_clock", request.ClockMeasureType.GetSubCommandFor()};
        var processStartInfo = ProcessStartInfoFactory.Create(Executables.VcGenCommand, args);
        // ReSharper disable once ConvertClosureToMethodGroup
        var frequency = await ProcessRunner.RunAsync(processStartInfo, value => ParseVcGenCommandOutput(value));
        this.logger.LogInformation("Measured {Frequency} for {Type}", frequency.Megahertz, request.ClockMeasureType.ToString());
        return new ClockMeasureResponse(request.ClockMeasureType.ToString(), frequency.Megahertz);
    }

    private static Frequency ParseVcGenCommandOutput(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        return Frequency.FromHertz(uint.Parse(value.Split('=')[1], NumberStyles.Integer, CultureInfo.InvariantCulture));
    }
}