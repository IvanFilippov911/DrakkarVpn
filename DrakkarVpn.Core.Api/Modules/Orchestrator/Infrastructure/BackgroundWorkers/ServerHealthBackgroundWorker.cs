using System.Collections.Concurrent;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.CleanupServerMetricsHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersForHealthPoll;
using DrakkarVpn.Shared;
using MediatR;

public sealed class ServerHealthBackgroundWorker : BackgroundService
{
    private readonly ILogger<ServerHealthBackgroundWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ConcurrentDictionary<Guid, int> _lastPeersCount = new();
    private int _loop = 0;

    private const int DelaySeconds = 10;
    private const int CleanupEveryLoops = 360;

    public ServerHealthBackgroundWorker(
        ILogger<ServerHealthBackgroundWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var servers = await mediator.Send(new GetServersForHealthPollRequest(), ct);
                
                var live = servers.Select(x => x.Id).ToHashSet();
                foreach (var k in _lastPeersCount.Keys)
                    if (!live.Contains(k)) _lastPeersCount.TryRemove(k, out _);

                foreach (var s in servers)
                {
                    AgentMetricsDto metrics;
                    try
                    {
                        metrics = await client.GetFromJsonAsync<AgentMetricsDto>(
                            $"{s.AgentBaseUrl}metrics", ct) ?? AgentMetricsDto.Empty;
                    }
                    catch
                    {
                        metrics = AgentMetricsDto.Empty;
                    }

                    if (metrics.Reachable)
                        _lastPeersCount.AddOrUpdate(s.Id, metrics.PeersActive, (_, __) => metrics.PeersActive);
                    else
                        metrics = metrics with { PeersActive = GetLastPeersSafe(s.Id) };
                    
                    await mediator.Send(new EvaluateServerHealthRequest(
                        s.Id,
                        metrics.Reachable,
                        metrics.PeersActive,
                        metrics.TrafficRxBytes,
                        metrics.TrafficTxBytes,
                        metrics.InfraLatencyMs,
                        metrics.VpnSpeedMbps
                    ), ct);
                }

                _loop++;
                if (_loop % CleanupEveryLoops == 0)
                {
                    await mediator.Send(new CleanupServerMetricsHistoryRequest(TimeSpan.FromHours(48)), ct);
                    _logger.LogInformation("ServerMetricsHistory: cleaned entries older than 48h");
                }
            }
            catch (OperationCanceledException) { Console.WriteLine("Ошибка операции"); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _logger.LogError(ex, "Health check error");
            }

            await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), ct);
        }
    }

    private int GetLastPeersSafe(Guid serverId) =>
        _lastPeersCount.TryGetValue(serverId, out var peers) ? peers : 0;
}
