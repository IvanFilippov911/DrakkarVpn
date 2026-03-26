using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Services;
using DrakkarVpn.Users.Application.DTOs;
using DrakkarVpn.Users.Application.DTOs.Devices;

namespace DrakkarVpn.Users.Application.Services;

public sealed class ConnectDeviceService : IConnectDeviceService
{
    private readonly ITelegramSessionValidationService _telegramSessionValidation;
    private readonly IDeviceSessionService _deviceSession;
    private readonly IJwtTokenService _jwt;
    private readonly IUsersQueryService _usersQuery;

    public ConnectDeviceService(
        ITelegramSessionValidationService telegramSessionValidation,
        IDeviceSessionService deviceSession,
        IJwtTokenService jwt,
        IUsersQueryService usersQuery)
    {
        _telegramSessionValidation = telegramSessionValidation;
        _deviceSession = deviceSession;
        _jwt = jwt;
        _usersQuery = usersQuery;
    }

    public async Task<ConnectDeviceResultDto> ConnectAsync(ConnectDeviceInput input, CancellationToken ct)
    {
        var session = await _telegramSessionValidation.ValidateAsync(input.InitData, ct);

        var user = await _usersQuery.GetByTelegramIdAsync(session.TelegramId, ct)
                   ?? throw new InvalidOperationException("User not found");
        
        var nowUtc = DateTime.UtcNow;

        var deviceId = await _deviceSession.GetOrCreateDeviceIdAsync(
            user.Id,
            input,
            nowUtc,
            ct);

        var token = _jwt.IssueToken(
            telegramId: session.TelegramId,
            deviceId: deviceId,
            ttl: TimeSpan.FromDays(30),
            platform: input.Platform,
            deviceName: input.DeviceName
        );

        return new ConnectDeviceResultDto(token, deviceId);
    }
}
