using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetTariffById;

public sealed record GetTariffByIdRequest(Guid TariffId) : IRequest<TariffDto?>;