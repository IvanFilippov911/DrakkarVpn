using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Security;

public sealed class InMemoryReplayStore : IReplayStore
{
    private readonly IMemoryCache _cache;
    public InMemoryReplayStore(IMemoryCache cache) => _cache = cache;

    public Task<bool> TryReserveAsync(string key, TimeSpan ttl, CancellationToken ct)
    {
        var cacheKey = $"replay:{key}";
        if (_cache.TryGetValue(cacheKey, out _))
            return Task.FromResult(false);

        _cache.Set(cacheKey, 1, ttl);
        return Task.FromResult(true);
    }
}
