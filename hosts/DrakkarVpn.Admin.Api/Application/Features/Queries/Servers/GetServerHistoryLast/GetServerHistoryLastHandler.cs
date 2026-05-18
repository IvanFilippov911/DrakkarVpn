using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerHistoryLast;

public sealed class GetServerHistoryLastHandler
    : IRequestHandler<GetServerHistoryLastRequest, IReadOnlyList<ServerMetricsHistoryDto>>
{
    private readonly IServerMetricsQueryService _serverMetrics;

    public GetServerHistoryLastHandler(IServerMetricsQueryService serverMetrics)
        => _serverMetrics = serverMetrics;

    public Task<IReadOnlyList<ServerMetricsHistoryDto>> Handle(
        GetServerHistoryLastRequest q,
        CancellationToken ct)
        => _serverMetrics.GetHistoryLastAsync(q.ServerId, q.Minutes, ct);
}