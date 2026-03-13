using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared.Errors;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Users.Application.Features.Services;

public sealed class ConnectDeviceService : IConnectDeviceService
{
    private readonly ITelegramInitDataValidator _twa;
    private readonly IReplayStore _replay;
    private readonly IJwtTokenService _jwt;
    private readonly IDeviceRepository _devices;
    private readonly IUsersQueryService _usersQuery;
    private readonly ISubscriptionQueryService _subsQuery;
    private readonly IDeviceIdGenerator _deviceIds;
    private readonly ILogger<ConnectDeviceService> _log;

    public ConnectDeviceService(
        ITelegramInitDataValidator twa,
        IReplayStore replay,
        IJwtTokenService jwt,
        IDeviceRepository devices,
        IUsersQueryService usersQuery,
        ISubscriptionQueryService subsQuery,
        IDeviceIdGenerator deviceIds,
        ILogger<ConnectDeviceService> log)
    {
        _twa        = twa;
        _replay     = replay;
        _jwt        = jwt;
        _devices    = devices;
        _usersQuery = usersQuery;
        _subsQuery  = subsQuery;
        _deviceIds  = deviceIds;
        _log        = log;
    }

    public async Task<ConnectDeviceResultDto> ConnectAsync(ConnectDeviceInput input, CancellationToken ct)
    {
        long telegramId;
        long authDateUnix;
        string? queryId;

        try
        {
            (telegramId, authDateUnix, queryId) = _twa.ValidateAndExtractAll(input.InitData);

            if (authDateUnix <= 0)
                throw new UnauthorizedAccessException("No auth_date");

            var authDate = DateTimeOffset.FromUnixTimeSeconds(authDateUnix).UtcDateTime;
            if (DateTime.UtcNow - authDate > TimeSpan.FromMinutes(5))
                throw new UnauthorizedAccessException("initData is stale");

            var replayKey = !string.IsNullOrWhiteSpace(queryId)
                ? $"qid:{queryId}"
                : $"tg:{telegramId}:ts:{authDateUnix}";

            var ok = await _replay.TryReserveAsync(replayKey, TimeSpan.FromMinutes(5), ct);
            if (!ok)
                throw new UnauthorizedAccessException("Replay detected");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException)
        {
            _log.LogWarning(ex, "InitData validation failed");
            throw;
        }

        var user = await _usersQuery.GetByTelegramIdAsync(telegramId, ct)
                   ?? throw new InvalidOperationException("User not found");

        if (user.Status == UserStatus.Banned)
            throw new UserBannedException(user.Id);

        var nowUtc = DateTime.UtcNow;

        var sub = await _subsQuery.GetActiveByUserAsync(user.Id, nowUtc, ct);
        if (sub is null)
            throw new InvalidOperationException("No active subscription");

        var used = await _usersQuery.CountActiveDevicesByUserAsync(user.Id, ct);
        if (used >= sub.MaxDevices)
            throw new InvalidOperationException("Device limit reached");

        string deviceId;

        if (!string.IsNullOrWhiteSpace(input.ExistingDeviceId))
        {
            var owns = await _devices.OwnsAsync(user.Id, input.ExistingDeviceId!, ct);
            if (!owns)
                throw new UnauthorizedAccessException("device does not belong to this user or is revoked");

            deviceId = input.ExistingDeviceId!;
            await _devices.UpdateAsync(deviceId, input.DeviceName, input.Platform, ct);
        }
        else
        {
            deviceId = _deviceIds.Generate();

            var device = Device.Create(
                deviceId:  deviceId,
                userId:    user.Id,
                name:      input.DeviceName,
                platform:  input.Platform,
                nowUtc:    nowUtc);

            await _devices.AddAsync(device, ct);
        }

        var token = _jwt.IssueToken(
            telegramId: telegramId,
            deviceId:   deviceId,
            ttl:        TimeSpan.FromDays(30),
            platform:   input.Platform,
            deviceName: input.DeviceName
        );

        return new ConnectDeviceResultDto(token, deviceId);
    }
}