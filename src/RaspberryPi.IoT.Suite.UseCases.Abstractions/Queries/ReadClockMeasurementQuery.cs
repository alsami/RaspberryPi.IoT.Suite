using MediatR;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Enums;
using RaspberryPi.IoT.Suite.UseCases.Abstractions.Response;

namespace RaspberryPi.IoT.Suite.UseCases.Abstractions.Queries;

public record ReadClockMeasurementQuery(ClockMeasureType ClockMeasureType) : IRequest<ClockMeasureResponse>;