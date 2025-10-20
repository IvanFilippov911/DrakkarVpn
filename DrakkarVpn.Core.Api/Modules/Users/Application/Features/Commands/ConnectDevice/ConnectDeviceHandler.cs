using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ConnectDevice;

public sealed class ConnectDeviceHandler : IRequestHandler<ConnectDeviceRequest, ConnectDeviceResponse>
{
    private readonly ITelegramInitDataValidator _twa;
    private readonly IReplayStore _replay;
    private readonly IJwtTokenService _jwt;
    private readonly IDeviceRepository _devices;
    private readonly IAppUserRepository _users;
    private readonly IDeviceIdGenerator _deviceIds;
    private readonly ILogger<ConnectDeviceHandler> _log;
    
    public ConnectDeviceHandler(
        ITelegramInitDataValidator twa,
        IReplayStore replay,
        IJwtTokenService jwt,
        IDeviceRepository devices,
        IAppUserRepository users,
        IDeviceIdGenerator deviceIds,
        ILogger<ConnectDeviceHandler> log)
    {
        _twa = twa;
        _replay = replay;
        _jwt = jwt;
        _devices = devices;
        _users = users;
        _deviceIds = deviceIds;
        _log = log;
    }

    public async Task<ConnectDeviceResponse> Handle(ConnectDeviceRequest req, CancellationToken ct)
    {
        long telegramId;
        long authDateUnix;
        string? queryId;

        try
        {
            (telegramId, authDateUnix, queryId) = _twa.ValidateAndExtractAll(req.InitData);

            if (authDateUnix <= 0)
                throw new UnauthorizedAccessException("No auth_date");

            var authDate = DateTimeOffset.FromUnixTimeSeconds(authDateUnix).UtcDateTime;
            
            if (DateTime.UtcNow - authDate > TimeSpan.FromMinutes(5))
                throw new UnauthorizedAccessException("initData is stale");

            var replayKey = !string.IsNullOrWhiteSpace(queryId)
                ? $"qid:{queryId}"
                : $"tg:{telegramId}:ts:{authDateUnix}";

            var ok = await _replay.TryReserveAsync(replayKey, TimeSpan.FromMinutes(5), ct);
            if (!ok) throw new UnauthorizedAccessException("Replay detected");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException)
        {
            _log.LogWarning(ex, "InitData validation failed");
            throw;
        }

        
        var user = await _users.GetByTelegramIdAsync(new TelegramId(telegramId), ct);
        string deviceId;

        if (!string.IsNullOrWhiteSpace(req.ExistingDeviceId))
        {
            var owns = await _devices.OwnsAsync(user.Id, req.ExistingDeviceId!, ct);
            if (!owns)
                throw new UnauthorizedAccessException("device does not belong to this user or is revoked");

            deviceId = req.ExistingDeviceId!;
            await _devices.UpdateAsync(deviceId, req.DeviceName, req.Platform, ct);
        }
        else
        {
            deviceId = _deviceIds.Generate();

            var device = Device.Create(
                deviceId: deviceId,
                userId:   user.Id,
                name:     req.DeviceName,
                platform: req.Platform,
                nowUtc:   DateTime.UtcNow
            );

            await _devices.AddAsync(device, ct);
        }
        
        var token = _jwt.IssueToken(
            telegramId: telegramId,
            deviceId:   deviceId,
            ttl:        TimeSpan.FromDays(30),
            platform:   req.Platform,
            deviceName: req.DeviceName
        );

        return new ConnectDeviceResponse(token, deviceId);
    }
}
