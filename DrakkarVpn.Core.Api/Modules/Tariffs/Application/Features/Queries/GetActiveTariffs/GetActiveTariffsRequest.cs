using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetActiveTariffs;

public sealed record GetActiveTariffsRequest() : IRequest<IReadOnlyList<TariffDto>>;