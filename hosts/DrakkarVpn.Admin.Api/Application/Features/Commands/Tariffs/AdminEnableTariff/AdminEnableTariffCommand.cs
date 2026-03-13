using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminEnableTariff;

public sealed record AdminEnableTariffCommand(Guid TariffId) : IRequest<Unit>, ITariffsCommand<Unit>;