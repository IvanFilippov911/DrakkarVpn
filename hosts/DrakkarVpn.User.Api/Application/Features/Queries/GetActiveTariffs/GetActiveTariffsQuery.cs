using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetActiveTariffs;

public sealed record GetActiveTariffsQuery : IRequest<IReadOnlyList<TariffDto>>;
