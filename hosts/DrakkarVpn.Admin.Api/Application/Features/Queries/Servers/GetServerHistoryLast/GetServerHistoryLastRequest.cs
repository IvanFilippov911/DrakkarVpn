using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerHistoryLast;

public sealed record GetServerHistoryLastRequest(
    Guid ServerId,
    int Minutes
) : IRequest<IReadOnlyList<ServerMetricsHistoryDto>>;