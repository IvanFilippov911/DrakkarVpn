using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Services;

public sealed class ServerPollStateService : IServerPollStateService
{
    private readonly IServerPollStateRepository _repo;

    private const int MaxBatchSize = 500;

    private static readonly TimeSpan MinLease = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaxLease = TimeSpan.FromMinutes(5);

    private static readonly TimeSpan MinStuck = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MaxStuck = TimeSpan.FromMinutes(30);

    private const int MaxErrorCodeLen = 64;

    public ServerPollStateService(IServerPollStateRepository repo) => _repo = repo;

    public Task<IReadOnlyList<ServerPollCandidateDto>> AcquireBatchAsync(
        DateTime nowUtc,
        int batchSize,
        TimeSpan leaseDuration,
        TimeSpan stuckTimeout,
        string instanceId,
        CancellationToken ct)
    {
        ValidateInstanceId(instanceId);

        if (batchSize <= 0)
            return Task.FromResult<IReadOnlyList<ServerPollCandidateDto>>([]);

        if (batchSize > MaxBatchSize)
            batchSize = MaxBatchSize;

        nowUtc = EnsureUtc(nowUtc);

        leaseDuration = Clamp(leaseDuration, MinLease, MaxLease);
        stuckTimeout  = Clamp(stuckTimeout,  MinStuck, MaxStuck);

        return _repo.AcquireBatchAsync(nowUtc, batchSize, leaseDuration, stuckTimeout, instanceId, ct);
    }

    public Task<ApplyPollStateResultDto> ApplyResultsAsync(
        IReadOnlyCollection<ServerPollResultDto> results,
        DateTime nowUtc,
        string instanceId,
        CancellationToken ct)
    {
        ValidateInstanceId(instanceId);

        if (results is null)
            throw new ArgumentNullException(nameof(results));

        if (results.Count == 0)
            return Task.FromResult(new ApplyPollStateResultDto([], []));

        nowUtc = EnsureUtc(nowUtc);

        var normalized = NormalizeResults(results);
        if (normalized.Count == 0)
            return Task.FromResult(new ApplyPollStateResultDto([], []));

        return _repo.ApplyResultsAsync(normalized, nowUtc, instanceId, ct);
    }
    

    private static void ValidateInstanceId(string instanceId)
    {
        if (string.IsNullOrWhiteSpace(instanceId))
            throw new ArgumentException("instanceId is required", nameof(instanceId));
    }

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    private static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
        => value < min ? min : (value > max ? max : value);

    private static List<ServerPollResultDto> NormalizeResults(IReadOnlyCollection<ServerPollResultDto> results)
    {
        return results
            .Where(r => r.ServerId != Guid.Empty)
            .GroupBy(r => r.ServerId)
            .Select(g => g.Last())
            .Select(NormalizeOne)
            .ToList();

        static ServerPollResultDto NormalizeOne(ServerPollResultDto r)
        {
            var peers = r.PeersActive < 0 ? 0 : r.PeersActive;
            var http  = r.HttpLatencyMs < 0 ? 0 : r.HttpLatencyMs;
            var rx    = r.RxTotal < 0 ? 0 : r.RxTotal;
            var tx    = r.TxTotal < 0 ? 0 : r.TxTotal;

            var code = string.IsNullOrWhiteSpace(r.ErrorCode)
                ? null
                : (r.ErrorCode!.Length > MaxErrorCodeLen ? r.ErrorCode[..MaxErrorCodeLen] : r.ErrorCode);

            return r with
            {
                PeersActive   = peers,
                HttpLatencyMs = http,
                RxTotal       = rx,
                TxTotal       = tx,
                ErrorCode     = code
            };
        }
    }
}