using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.PullServerMetrics;

public sealed record PullServerMetricsCommand(Guid ServerId) : IRequest<Unit>;