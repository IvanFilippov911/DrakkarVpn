using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistoryLast;

public sealed record GetServerHistoryLastRequest(
    Guid ServerId,
    int Minutes = 1440
) : IRequest<IReadOnlyList<ServerMetricsHistoryDto>>;