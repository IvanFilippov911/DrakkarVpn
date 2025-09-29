using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.EnableTariff;

public sealed record EnableTariffRequest(Guid TariffId) : IRequest<Unit>;