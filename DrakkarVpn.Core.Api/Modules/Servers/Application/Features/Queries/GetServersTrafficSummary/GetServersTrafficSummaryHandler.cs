using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersTrafficSummary;

public sealed class GetServersTrafficSummaryHandler
    : IRequestHandler<GetServersTrafficSummaryRequest, Dictionary<Guid, long>>
{
    private readonly IServerMetricsHistoryRepository _history;

    public GetServersTrafficSummaryHandler(IServerMetricsHistoryRepository history)
    {
        _history = history;
    }

    public async Task<Dictionary<Guid, long>> Handle(
        GetServersTrafficSummaryRequest req,
        CancellationToken ct)
    {
        if (req.ServerIds is null || req.ServerIds.Length == 0)
            return new();
        
        var traffic = await _history.GetTrafficSumAsync(
            req.ServerIds,
            req.FromUtc,
            ct);

        return traffic;
    }
}