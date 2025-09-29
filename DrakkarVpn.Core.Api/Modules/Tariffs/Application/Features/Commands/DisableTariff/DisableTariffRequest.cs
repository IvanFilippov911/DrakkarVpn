using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.DisableTariff;

public sealed record DisableTariffRequest(Guid TariffId) : IRequest<Unit>;