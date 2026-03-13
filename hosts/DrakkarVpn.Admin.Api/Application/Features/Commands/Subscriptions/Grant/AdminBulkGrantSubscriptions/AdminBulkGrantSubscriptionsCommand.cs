using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkGrantSubscriptions;

public sealed record AdminBulkGrantSubscriptionsCommand(
    IReadOnlyList<Guid> UserIds,
    Guid TariffId,
    int? DeviceCount
) : IRequest<BulkGrantSubscriptionsResponse>, ISubscriptionsCommand<BulkGrantSubscriptionsResponse>;