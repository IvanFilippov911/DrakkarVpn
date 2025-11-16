using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.UpdateTariff;

public sealed record UpdateTariffRequest(
    Guid TariffId,
    string Name,
    decimal Price,
    TimeSpan Duration,
    int DefaultMaxDevices
) : IRequest<Unit>;