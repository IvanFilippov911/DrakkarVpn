using System.Text.Json;
using StackExchange.Redis;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreHealthHistory;

public static class RedisJson
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T? DeserializeOrDefault<T>(
        this RedisValue value,
        JsonSerializerOptions? options = null)
    {
        if (value.IsNullOrEmpty) return default;

        options ??= DefaultOptions;

        try
        {
            return JsonSerializer.Deserialize<T>(value!, options);
        }
        catch
        {
            return default;
        }
    }
}