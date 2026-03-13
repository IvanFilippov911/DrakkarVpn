using DrakkarVpn.Core.Api.Diagnostics;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreHealth;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreHealthHistory;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using StackExchange.Redis;

namespace DrakkarVpn.Observability.Application.Services.Diagnostics;

public sealed class CoreHealthQueryService : ICoreHealthQueryService
{
    private readonly IDatabase _redis;

    private const string SummaryKey  = "drakkar:core:metrics:summary";
    private const string HistoryKey  = "drakkar:core:metrics:history";
    private const int    DefaultLimit = 36;

    public CoreHealthQueryService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }
    

    public async Task<CoreHealthDto> GetSnapshotAsync(CancellationToken ct)
    {
        var hash = await _redis.HashGetAllAsync(SummaryKey);
        if (hash.Length == 0)
            return CoreHealthDto.Empty();

        var map = hash.ToStringDictionary();

        var errorsByArea = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var h in hash)
        {
            var name = h.Name.ToString();

            if (name.StartsWith("Errors.", StringComparison.OrdinalIgnoreCase) &&
                double.TryParse(h.Value.ToString(), out var value))
            {
                var area = name["Errors.".Length..];
                errorsByArea[area] = value;
            }
        }

        return new CoreHealthDto(
            Rps:            CoreHealthRedisParser.GetDouble(map, "Rps"),
            AvgLatencyMs:   CoreHealthRedisParser.GetDouble(map, "AvgLatencyMs"),
            ErrorRatePct:   CoreHealthRedisParser.GetDouble(map, "ErrorRatePct"),
            TotalRequests:  CoreHealthRedisParser.GetLong(map, "TotalRequests"),
            TotalErrors:    CoreHealthRedisParser.GetLong(map, "TotalErrors"),
            WindowStartUtc: CoreHealthRedisParser.GetDate(map, "WindowStartUtc"),
            WindowEndUtc:   CoreHealthRedisParser.GetDate(map, "WindowEndUtc"),
            ErrorsByArea:   errorsByArea
        );
    }

    

    public async Task<IReadOnlyList<CoreHealthHistoryPointDto>> GetHistoryAsync(
        int? limit,
        CancellationToken ct)
    {
        var take = limit is null or <= 0
            ? DefaultLimit
            : limit.Value;

        var values = await _redis.ListRangeAsync(HistoryKey, -take, -1);

        if (values.Length == 0)
            return Array.Empty<CoreHealthHistoryPointDto>();

        var result = new List<CoreHealthHistoryPointDto>(values.Length);

        foreach (var val in values)
        {
            var entry = RedisJson.DeserializeOrDefault<CoreMetricsHistoryEntry>(val);
            if (entry is null)
                continue;

            result.Add(new CoreHealthHistoryPointDto(
                TimestampUtc: entry.TimestampUtc,
                Rps:          entry.Rps,
                AvgLatencyMs: entry.AvgLatencyMs,
                ErrorRatePct: entry.ErrorRatePct,
                TotalErrors:  entry.TotalErrors,
                ErrorsByArea: entry.ErrorsByArea ?? new Dictionary<string, long>()
            ));
        }

        return result;
    }
}