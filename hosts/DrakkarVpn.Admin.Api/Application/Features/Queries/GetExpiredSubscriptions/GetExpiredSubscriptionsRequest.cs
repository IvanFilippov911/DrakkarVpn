using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;

public sealed record GetExpiredSubscriptionsRequest : IRequest<IReadOnlyList<Guid>>;