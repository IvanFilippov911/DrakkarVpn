using System.Collections.Concurrent;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpsertServerMetricsHistory;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.PullServerMetrics;

public sealed class PullServerMetricsHandler
    : IRequestHandler<PullServerMetricsCommand, Unit>
{
    private readonly IServerRepository _repository;
    private readonly IHttpClientFactory _http;
    private readonly IMediator _mediator;
    private readonly ILogger<PullServerMetricsHandler> _logger;
    
    private static readonly ConcurrentDictionary<Guid, int> _lastPeers = new();

    public PullServerMetricsHandler(
        IServerRepository repository,
        IHttpClientFactory http,
        IMediator mediator,
        ILogger<PullServerMetricsHandler> logger)
    {
        _repository = repository;
        _http = http;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Unit> Handle(PullServerMetricsCommand cmd, CancellationToken ct)
    {
        var server = await _repository.GetAsync(cmd.ServerId, ct);

        if (server is null)
        {
            _logger.LogWarning("Server {ServerId} not found for health poll", cmd.ServerId);
            return Unit.Value;
        }

        var client = _http.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        AgentMetricsDto metrics;
        try
        {
            metrics = await client.GetFromJsonAsync<AgentMetricsDto>(
                         $"{server.AgentBaseUrl}metrics", ct)
                      ?? AgentMetricsDto.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Agent unreachable for server {ServerId}", cmd.ServerId);
            metrics = AgentMetricsDto.Empty;
        }

        if (metrics.Reachable)
        {
            _lastPeers.AddOrUpdate(cmd.ServerId, metrics.PeersActive, (_, _) => metrics.PeersActive);
        }
        else if (_lastPeers.TryGetValue(cmd.ServerId, out var last))
        {
            metrics = metrics with { PeersActive = last };
        }

        
        await _mediator.Send(new EvaluateServerHealthRequest(
            cmd.ServerId,
            metrics.Reachable,
            metrics.PeersActive,
            metrics.TrafficRxBytes,
            metrics.TrafficTxBytes,
            metrics.InfraLatencyMs,
            metrics.VpnSpeedMbps
        ), ct);
        
        
        var slot10s = TruncateTo10sUtc(DateTime.UtcNow);

        await _mediator.Send(new UpsertServerMetricsHistoryRequest(
            ServerId:       cmd.ServerId,
            PeriodStartUtc: slot10s,
            Reachable:      metrics.Reachable,
            TrafficRxBytes: metrics.TrafficRxBytes,
            TrafficTxBytes: metrics.TrafficTxBytes,
            VpnSpeedMbps:   (decimal)metrics.VpnSpeedMbps,
            InfraLatencyMs: (decimal)metrics.InfraLatencyMs
        ), ct);

        return Unit.Value;
    }

    private static DateTime TruncateTo10sUtc(DateTime utc)
    {
        var sec = utc.Second - (utc.Second % 10);
        return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, sec, DateTimeKind.Utc);
    }
}