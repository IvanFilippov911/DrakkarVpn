using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkGrantSubscriptions;

public sealed class AdminBulkGrantSubscriptionsHandler
    : IRequestHandler<AdminBulkGrantSubscriptionsCommand, BulkGrantSubscriptionsResponse>
{
    private readonly ISubscriptionGrantService _subs;
    private readonly IAppUserReadStore _usersRead;

    public AdminBulkGrantSubscriptionsHandler(
        ISubscriptionGrantService subs,
        IAppUserReadStore usersRead)
    {
        _subs      = subs;
        _usersRead = usersRead;
    }

    public async Task<BulkGrantSubscriptionsResponse> Handle(AdminBulkGrantSubscriptionsCommand cmd, CancellationToken ct)
    {
        var markerUtc = DateTime.UtcNow;

        var inputIds = Ids.NormalizeDistinct(cmd.UserIds);
        if (inputIds.Count == 0)
            return BulkGrantSubscriptionsResponse.Empty();

        var existingIds = await _usersRead.GetExistingIdsAsync(inputIds, ct);
        if (existingIds.Count == 0)
            return BulkGrantSubscriptionsResponse.OnlyNotFound(inputIds);

        var existingSet = existingIds.ToHashSet();
        var notFound = inputIds.Where(id => !existingSet.Contains(id)).ToList();

        var res = await _subs.GrantBulkAsync(existingIds, cmd.TariffId, cmd.DeviceCount, markerUtc, ct);

        return BulkGrantSubscriptionsResponse.From(res, notFound);
    }
    
    internal static class Ids
    {
        public static List<Guid> NormalizeDistinct(IReadOnlyCollection<Guid> input)
            => input.Where(x => x != Guid.Empty).Distinct().ToList();
    }
}