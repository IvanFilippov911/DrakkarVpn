using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Services;

public sealed class HomeContextService : IHomeContextService
{
    private readonly IUsersQueryService _users;
    private readonly ISubscriptionQueryService _subs;
    private readonly IPeersQueryService _peers;
    private readonly IPeerProvisionJobsService _jobs;

    public HomeContextService(
        IUsersQueryService users,
        ISubscriptionQueryService subs,
        IPeersQueryService peers,
        IPeerProvisionJobsService jobs)
    {
        _users = users;
        _subs = subs;
        _peers = peers;
        _jobs = jobs;
    }

    public async Task<HomeContextDto> GetAsync(
        long telegramId,
        string deviceId,
        CancellationToken ct)
    {
        deviceId = deviceId.Trim();

        var user = await _users.GetByTelegramIdAsync(telegramId, ct)
                   ?? throw new InvalidOperationException("User not found");

        if (user.Status == UserStatus.Banned)
            return new HomeContextDto(
                State: HomeScreenStateDto.Blocked,
                PendingProvisionJobId: null,
                PollUrl: null);

        var nowUtc = DateTime.UtcNow;
        var sub = await _subs.GetActiveByUserAsync(user.Id, nowUtc, ct);
        if (sub is null)
            return new HomeContextDto(
                State: HomeScreenStateDto.NoSubscription,
                PendingProvisionJobId: null,
                PollUrl: null);
        
        var countDevice = await _users.CountActiveDevicesByUserAsync(user.Id, ct);
        if (countDevice > sub.MaxDevices)
            return new HomeContextDto(
                State: HomeScreenStateDto.DeviceLimitExceeded,
                PendingProvisionJobId: null,
                PollUrl: null);
        
        var jobId = await _jobs.GetActiveJobIdByDeviceIdAsync(deviceId, ct);
        if (jobId is not null)
        {
            var pollUrl = $"/api/v1/user/vpn/config/provision/{jobId}";
            return new HomeContextDto(
                State: HomeScreenStateDto.Pending,
                PendingProvisionJobId: jobId,
                PollUrl: pollUrl);
        }
        
        var peer = await _peers.GetActivePeerForDeviceAsync(deviceId, ct);
        if (peer is not null)
            return new HomeContextDto(
                State: HomeScreenStateDto.Ready,
                PendingProvisionJobId: null,
                PollUrl: null);

        return new HomeContextDto(
            State: HomeScreenStateDto.NotStarted,
            PendingProvisionJobId: null,
            PollUrl: null);
    }
}
