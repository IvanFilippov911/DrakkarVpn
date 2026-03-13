using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Shared.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Telegram;

public sealed class TelegramInitDataValidator : ITelegramInitDataValidator
{
    private readonly ILogger<TelegramInitDataValidator> _logger;
    private readonly byte[] _secretKey;
    private const int MaxInitDataLength = 16_384;

    public TelegramInitDataValidator(
        IOptions<TelegramOptions> options,
        ILogger<TelegramInitDataValidator> logger)
    {
        _logger = logger;

        var botToken = options.Value.BotToken;
        _secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("WebAppData"),
            Encoding.UTF8.GetBytes(botToken));
    }
    
    public (long TelegramId, long AuthDateUnix, string? QueryId) ValidateAndExtractAll(string initData)
    {
        if (string.IsNullOrWhiteSpace(initData))
        {
            _logger.LogWarning("initData is null or empty");
            throw new ArgumentException("initData cannot be null or empty", nameof(initData));
        }
        if (initData.Length > MaxInitDataLength)
        {
            _logger.LogWarning("initData too large: {Len}", initData.Length);
            throw new UnauthorizedAccessException("initData too large");
        }

        try
        {
            var dict = ParseToDictionary(initData, out var hashProvided);
            var dataCheckString = BuildDataCheckString(dict);
            VerifyHash(hashProvided, dataCheckString);

            var telegramId = ExtractTelegramId(dict);
            var authDate   = TryParseAuthDate(dict);
            var queryId    = dict.TryGetValue("query_id", out var qid) ? qid.ToString() : null;

            _logger.LogInformation("initData validated for TelegramId {TelegramId}", telegramId);
            return (telegramId, authDate, queryId);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "initData validation failed");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Bad user JSON format");
            throw new UnauthorizedAccessException("Bad user JSON format", ex);
        }
    }
    
    private static IReadOnlyDictionary<string, StringValues> ParseToDictionary(string initData, out string hash)
    {
        var q = QueryHelpers.ParseQuery(initData);

        if (!q.TryGetValue("hash", out var hv))
            throw new UnauthorizedAccessException("No hash");

        hash = hv.ToString();
        return q; 
    }

    private static string BuildDataCheckString(IReadOnlyDictionary<string, StringValues> dict)
    {
        var sortedPairs = dict
            .Where(p => p.Key != "hash")
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .Select(p => $"{p.Key}={p.Value.ToString()}"); 

        return string.Join("\n", sortedPairs);
    }

    private void VerifyHash(string hashProvided, string dataCheckString)
    {
        if (hashProvided.Length != 64)
        {
            _logger.LogWarning("Invalid hash length: {Len}", hashProvided.Length);
            throw new UnauthorizedAccessException("Bad hash format");
        }
        
        Span<byte> mac = stackalloc byte[32];
        HMACSHA256.TryHashData(_secretKey, Encoding.UTF8.GetBytes(dataCheckString), mac, out _);
        
        Span<byte> provided = stackalloc byte[32];
        try
        {
            Convert.FromHexString(hashProvided).CopyTo(provided);
        }
        catch (FormatException)
        {
            throw new UnauthorizedAccessException("Bad hash format");
        }
        
        if (!CryptographicOperations.FixedTimeEquals(mac, provided))
            throw new UnauthorizedAccessException("Invalid hash");
    }

    private static long ExtractTelegramId(IReadOnlyDictionary<string, StringValues> dict)
    {
        if (!dict.TryGetValue("user", out var userJsonVals))
            throw new UnauthorizedAccessException("No user data");

        var userJson = userJsonVals.ToString();
        if (string.IsNullOrWhiteSpace(userJson))
            throw new UnauthorizedAccessException("No user data");

        using var doc = JsonDocument.Parse(userJson);
        if (!doc.RootElement.TryGetProperty("id", out var idProp) ||
            idProp.ValueKind != JsonValueKind.Number)
            throw new UnauthorizedAccessException("No user.id");

        return idProp.GetInt64();
    }

    private static long TryParseAuthDate(IReadOnlyDictionary<string, StringValues> dict)
        => (dict.TryGetValue("auth_date", out var v) && long.TryParse(v.ToString(), out var ts)) ? ts : 0;
}
