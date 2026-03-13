using System.Diagnostics;
using System.Net.Http.Json;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;
using DrakkarVpn.Shared;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class AgentPollingService : IAgentPollingService
{
    private readonly IHttpClientFactory _http;
    private readonly IServerRepository _servers;
    private readonly ILogger<AgentPollingService> _log;

    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(5);

    public AgentPollingService(
        IHttpClientFactory http,
        IServerRepository servers,
        ILogger<AgentPollingService> log)
    {
        _http = http;
        _servers = servers;
        _log = log;
    }

    public async Task<IReadOnlyList<ServerPollResultDto>> PollBatchAsync(
        IReadOnlyList<ServerPollCandidateDto> candidates,
        int httpConcurrency,
        CancellationToken ct)
    {
        if (candidates.Count == 0)
            return [];

        if (httpConcurrency <= 0)
            httpConcurrency = 1;

        using var semaphore = new SemaphoreSlim(httpConcurrency);
        var tasks = new List<Task<ServerPollResultDto>>(candidates.Count);

        foreach (var c in candidates)
        {
            tasks.Add(PollOneAsync(c, semaphore, ct));
        }

        return await Task.WhenAll(tasks);
    }
 
    private async Task<ServerPollResultDto> PollOneAsync(
        ServerPollCandidateDto c,
        SemaphoreSlim semaphore,
        CancellationToken ct)
    {
        await semaphore.WaitAsync(ct);
        try
        {
            return await PollOneInternalAsync(c, ct);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<ServerPollResultDto> PollOneInternalAsync(
        ServerPollCandidateDto c,
        CancellationToken ct)
    {
        var server = await _servers.GetAsync(c.ServerId, ct);
        if (server is null)
            return ServerPollResultDto.Failure(c, errorCode: "ServerNotFound");

        var client = _http.CreateClient();
        client.Timeout = HttpTimeout;

        var sw = Stopwatch.StartNew();

        try
        {
            var metrics = await client.GetFromJsonAsync<AgentMetricsDto>(
                $"{server.AgentBaseUrl}metrics",
                ct);

            sw.Stop();

            if (metrics is null)
                return ServerPollResultDto.Failure(c, (int)sw.ElapsedMilliseconds, "EmptyResponse");

            return new ServerPollResultDto(
                ServerId:       c.ServerId,
                Xmin:           c.Xmin,
                Reachable:      metrics.Reachable,
                PeersActive:    metrics.Reachable
                    ? metrics.PeersActive
                    : c.LastKnownPeersActive,
                RxTotal:        metrics.TrafficRxBytes,
                TxTotal:        metrics.TrafficTxBytes,
                InfraLatencyMs: metrics.InfraLatencyMs,
                VpnSpeedMbps:   metrics.VpnSpeedMbps,
                HttpLatencyMs:  (int)sw.ElapsedMilliseconds,
                Success:        true,
                ErrorCode:      null
            );
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            sw.Stop();
            return ServerPollResultDto.Failure(c, (int)sw.ElapsedMilliseconds, "Timeout");
        }
        catch (HttpRequestException)
        {
            sw.Stop();
            return ServerPollResultDto.Failure(c, (int)sw.ElapsedMilliseconds, "ConnectionFailed");
        }
        catch (Exception ex)
        {
            sw.Stop();
            _log.LogDebug(ex, "Agent poll failed for server {ServerId}", c.ServerId);
            return ServerPollResultDto.Failure(c, (int)sw.ElapsedMilliseconds, "UnhandledError");
        }
    }
}