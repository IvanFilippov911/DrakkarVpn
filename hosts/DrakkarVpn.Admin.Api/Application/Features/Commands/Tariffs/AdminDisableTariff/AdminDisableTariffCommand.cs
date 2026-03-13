using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminDisableTariff;

public sealed record AdminDisableTariffCommand(Guid TariffId) : IRequest<Unit>, ITariffsCommand<Unit>;