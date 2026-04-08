using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;

public sealed record AdminCreateTariffCommand(
    string Name,
    TimeSpan Duration,
    decimal Price,
    int DefaultMaxDevices,
    TariffKind Kind
) : IRequest<Guid>, ITariffsCommand<Guid>;