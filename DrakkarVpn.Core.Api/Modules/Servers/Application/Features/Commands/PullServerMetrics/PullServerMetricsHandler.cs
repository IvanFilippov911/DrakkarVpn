using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.PullServerMetrics;

public sealed class PullServerMetricsHandler
    : IRequestHandler<PullServerMetricsCommand, Unit>
{
    private readonly AppDbContext _db;
    private readonly IHttpClientFactory _http;
    private readonly IMediator _mediator;
    private readonly ILogger<PullServerMetricsHandler> _logger;
    
    private static readonly Dictionary<Guid, int> _lastPeers = new();

    public PullServerMetricsHandler(
        AppDbContext db,
        IHttpClientFactory http,
        IMediator mediator,
        ILogger<PullServerMetricsHandler> logger)
    {
        _db = db;
        _http = http;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Unit> Handle(PullServerMetricsCommand cmd, CancellationToken ct)
    {
        var server = await _db.Servers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == cmd.ServerId, ct);

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
            _lastPeers[cmd.ServerId] = metrics.PeersActive;
        }
        else
        {
            if (_lastPeers.TryGetValue(cmd.ServerId, out var last))
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

        return Unit.Value;
    }
}