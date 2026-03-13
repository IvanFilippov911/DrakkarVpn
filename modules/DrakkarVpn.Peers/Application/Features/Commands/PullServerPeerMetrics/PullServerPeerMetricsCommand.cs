using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.PullServerPeerMetrics;

public sealed record PullServerPeerMetricsCommand(Guid ServerId) : IRequest<Unit>;