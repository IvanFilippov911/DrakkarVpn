using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;

public sealed record GetActiveSubscriptionByUserRequest(Guid UserId) : IRequest<GetActiveSubscriptionDto>;