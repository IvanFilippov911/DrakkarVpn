using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.CreateTariff;

public sealed record CreateTariffRequest(
    string Name,
    decimal Price,
    TimeSpan Duration,
    int DefaultMaxDevices
) : IRequest<Guid>;