using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.CleanupServerMetricsHistory;

public sealed record CleanupServerMetricsHistoryRequest(TimeSpan Ttl) : IRequest<Unit>;