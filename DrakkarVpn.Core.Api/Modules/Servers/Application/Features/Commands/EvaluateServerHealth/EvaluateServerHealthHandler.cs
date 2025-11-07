using System.Collections.Concurrent;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpsertServerMetricsHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using MediatR;

public sealed class EvaluateServerHealthHandler 
    : IRequestHandler<EvaluateServerHealthRequest, bool>
{
    private readonly IServerRepository _repo;
    private readonly IMediator _mediator;

    private static readonly ConcurrentDictionary<Guid, int> _failCounts = new();

    public EvaluateServerHealthHandler(IServerRepository repo, IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    public async Task<bool> Handle(EvaluateServerHealthRequest req, CancellationToken ct)
    {
        var server = await _repo.GetAsync(req.ServerId, ct);
        if (server is null) return false;

        var newStatus = server.Status;

        if (!req.Reachable)
        {
            var cnt = _failCounts.AddOrUpdate(req.ServerId, 1, (_, v) => v + 1);
            if (cnt >= 3) newStatus = ServerStatus.Disabled;
            else if (server.Status == ServerStatus.Enabled) newStatus = ServerStatus.Draining;
        }
        else
        {
            _failCounts[req.ServerId] = 0;
            if (server.Status == ServerStatus.Disabled) newStatus = ServerStatus.Draining;
            else if (server.Status == ServerStatus.Draining) newStatus = ServerStatus.Enabled;
            if (req.PeersActive >= server.MaxPeers) newStatus = ServerStatus.Draining;
        }

        if (!server.IsSameState(req))
        {
            server.SetStatus(newStatus);
            server.UpdateHealth(req.Reachable, req.PeersActive);
            server.UpdateMetrics(req.TrafficRxBytes, req.TrafficTxBytes, req.VpnSpeedMbps, req.InfraLatencyMs);
        }
        
        return true;
    }

    private static DateTime TruncateTo10sUtc(DateTime utc)
    {
        var sec = utc.Second - (utc.Second % 10);
        return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, sec, DateTimeKind.Utc);
    }
}
