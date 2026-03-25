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
    private readonly ILogger<AgentPollingService> _log;

    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(5);

    public AgentPollingService(
        IHttpClientFactory http,
        ILogger<AgentPollingService> log)
    {
        _http = http;
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
        ServerPollCandidateDto candidate,
        CancellationToken ct)
    {
        var client = _http.CreateClient();
        client.Timeout = HttpTimeout;

        var sw = Stopwatch.StartNew();

        try
        {
            var metrics = await client.GetFromJsonAsync<AgentMetricsDto>(
                $"{candidate.AgentBaseUrl}metrics",
                ct);

            sw.Stop();

            if (metrics is null)
                return ServerPollResultDto.Failure(candidate, (int)sw.ElapsedMilliseconds, "EmptyResponse");

            return new ServerPollResultDto(
                ServerId:       candidate.ServerId,
                Xmin:           candidate.Xmin,
                Reachable:      metrics.Reachable,
                PeersActive:    metrics.Reachable
                    ? metrics.PeersActive
                    : candidate.LastKnownPeersActive,
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
            return ServerPollResultDto.Failure(candidate, (int)sw.ElapsedMilliseconds, "Timeout");
        }
        catch (HttpRequestException)
        {
            sw.Stop();
            return ServerPollResultDto.Failure(candidate, (int)sw.ElapsedMilliseconds, "ConnectionFailed");
        }
        catch (Exception ex)
        {
            sw.Stop();
            _log.LogDebug(ex, "Agent poll failed for server {ServerId}", candidate.ServerId);
            return ServerPollResultDto.Failure(candidate, (int)sw.ElapsedMilliseconds, "UnhandledError");
        }
    }
}