using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminUpdateTariff;

public sealed record AdminUpdateTariffCommand(
    Guid TariffId,
    string Name,
    TimeSpan Duration,
    decimal Price,
    int DefaultMaxDevices
) : IRequest<Unit>, ITariffsCommand<Unit>;