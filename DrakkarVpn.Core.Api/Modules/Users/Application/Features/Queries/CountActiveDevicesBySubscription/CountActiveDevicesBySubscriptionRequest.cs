using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.CountActiveDevicesBySubscription;

public sealed record CountActiveDevicesBySubscriptionRequest(Guid SubscriptionId) : IRequest<int>;