using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Shared.Errors;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Services;

public sealed class VpnAccessContextService : IVpnAccessContextService
{
    private readonly IUsersQueryService _usersQuery;
    private readonly ISubscriptionQueryService _subsQuery;

    public VpnAccessContextService(
        IUsersQueryService usersQuery,
        ISubscriptionQueryService subsQuery)
    {
        _usersQuery = usersQuery;
        _subsQuery = subsQuery;
    }

    public async Task<VpnAccessContextDto> GetVpnAccessContextAsync(
        long telegramId,
        string deviceId,
        CancellationToken ct)
    {
        var normalizedDeviceId = deviceId.Trim();
        var nowUtc = DateTime.UtcNow;

        var user = await _usersQuery.GetByTelegramIdAsync(telegramId, ct)
                   ?? throw new InvalidOperationException("User not found");

        if (user.Status == UserStatus.Banned)
            throw new UserBannedException(user.Id);

        var sub = await _subsQuery.GetActiveByUserAsync(user.Id, nowUtc, ct);
        if (sub is null)
            throw new InvalidOperationException("No active subscription");

        return new VpnAccessContextDto(
            UserId: user.Id,
            DeviceId: normalizedDeviceId,
            NowUtc: nowUtc,
            SubscriptionId: sub.Id,
            MaxDevices: sub.MaxDevices);
    }
}

