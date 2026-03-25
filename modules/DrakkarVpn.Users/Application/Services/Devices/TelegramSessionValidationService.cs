using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Services;
using DrakkarVpn.Users.Application.DTOs.Devices;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Users.Application.Services;

public sealed class TelegramSessionValidationService : ITelegramSessionValidationService
{
    private readonly ITelegramInitDataValidator _twa;
    private readonly IReplayStore _replay;
    private readonly ILogger<TelegramSessionValidationService> _log;

    public TelegramSessionValidationService(
        ITelegramInitDataValidator twa,
        IReplayStore replay,
        ILogger<TelegramSessionValidationService> log)
    {
        _twa = twa;
        _replay = replay;
        _log = log;
    }

    public async Task<TelegramSessionValidationResult> ValidateAsync(string initData, CancellationToken ct)
    {
        try
        {
            var (telegramId, authDateUnix, queryId) = _twa.ValidateAndExtractAll(initData);

            if (authDateUnix <= 0)
                throw new UnauthorizedAccessException("No auth_date");

            var authDateUtc = DateTimeOffset.FromUnixTimeSeconds(authDateUnix).UtcDateTime;
            if (DateTime.UtcNow - authDateUtc > TimeSpan.FromMinutes(5))
                throw new UnauthorizedAccessException("initData is stale");

            var replayKey = !string.IsNullOrWhiteSpace(queryId)
                ? $"qid:{queryId}"
                : $"tg:{telegramId}:ts:{authDateUnix}";

            var reserved = await _replay.TryReserveAsync(replayKey, TimeSpan.FromMinutes(5), ct);
            if (!reserved)
                throw new UnauthorizedAccessException("Replay detected");

            return new TelegramSessionValidationResult(
                TelegramId: telegramId,
                AuthDateUtc: authDateUtc,
                ReplayKey: replayKey);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException)
        {
            _log.LogWarning(ex, "InitData validation failed");
            throw;
        }
    }
}