using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetTariffById;

public sealed record AdminGetTariffByIdQuery(Guid TariffId) : IRequest<TariffDto?>;