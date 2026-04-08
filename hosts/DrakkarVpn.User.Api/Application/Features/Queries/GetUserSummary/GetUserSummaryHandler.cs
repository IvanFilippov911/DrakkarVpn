using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserSummary;

public sealed class GetUserSummaryHandler
    : IRequestHandler<GetUserSummaryRequest, UserSummaryDto>
{
    private readonly IUsersQueryService _users;
    private readonly ISubscriptionQueryService _subs;

    public GetUserSummaryHandler(
        IUsersQueryService users,
        ISubscriptionQueryService subs)
    {
        _users = users;
        _subs = subs;
    }

    public async Task<UserSummaryDto> Handle(GetUserSummaryRequest req, CancellationToken ct)
    {
        var user = await _users.GetByTelegramIdAsync(req.TelegramId, ct)
                   ?? throw new InvalidOperationException("User not found");

        var connected = await _users.CountActiveDevicesByUserAsync(user.Id, ct);

        var nowUtc = DateTime.UtcNow;
        var sub = await _subs.GetActiveByUserAsync(user.Id, nowUtc, ct);

        return new UserSummaryDto(
            SubscriptionEndAtUtc: sub?.EndAt,
            ConnectedDevices: connected,
            MaxDevices: sub?.MaxDevices
        );
    }
}

