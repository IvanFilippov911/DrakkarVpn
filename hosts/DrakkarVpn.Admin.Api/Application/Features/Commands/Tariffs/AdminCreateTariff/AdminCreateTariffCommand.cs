using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;

public sealed record AdminCreateTariffCommand(
    string Name,
    TimeSpan Duration,
    decimal Price,
    int DefaultMaxDevices
) : IRequest<Guid>, ITariffsCommand<Guid>;