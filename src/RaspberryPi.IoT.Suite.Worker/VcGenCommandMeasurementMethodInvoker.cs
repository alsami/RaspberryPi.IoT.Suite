using System.Net;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaspberryPi.IoT.Suite.Messages;
using RaspberryPi.IoT.Suite.Services.Abstractions;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Enums;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Queries;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Response;

namespace RaspberryPi.IoT.Suite.Worker;

public class VcGenCommandMeasurementMethodInvoker: BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IMethodHandler<ClockMeasurementMessage, ObjectMethodResponse<ClockMeasureResponse>> methodHandler;

    public VcGenCommandMeasurementMethodInvoker(IServiceProvider serviceProvider, IMethodHandler<ClockMeasurementMessage, ObjectMethodResponse<ClockMeasureResponse>> methodHandler)
    {
        this.serviceProvider = serviceProvider;
        this.methodHandler = methodHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.methodHandler.RegisterHandlerAsync(DeviceMethod.ClockMeasurement, this.MethodAccessor, stoppingToken);
    }

    private async Task<ObjectMethodResponse<ClockMeasureResponse>> MethodAccessor(ClockMeasurementMessage? arg)
    {
        if (arg is null)
        {
            return new ObjectMethodResponse<ClockMeasureResponse>(null, HttpStatusCode.BadRequest);
        }

        using var scope = this.serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new ReadClockMeasurementQuery(Enum.Parse<ClockMeasureType>(arg.ClockMeasureType, true));
        var response = await mediator.Send(query);
            
        return new ObjectMethodResponse<ClockMeasureResponse>(response, HttpStatusCode.OK);
    }
}