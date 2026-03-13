using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetActiveTariffs;

public sealed record AdminGetActiveTariffsQuery() : IRequest<IReadOnlyList<TariffDto>>;