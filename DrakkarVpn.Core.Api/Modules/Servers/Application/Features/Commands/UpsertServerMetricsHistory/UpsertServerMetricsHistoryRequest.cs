using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpsertServerMetricsHistory;

public sealed record UpsertServerMetricsHistoryRequest(
    Guid ServerId,
    DateTime PeriodStartUtc,
    bool Reachable,
    long TrafficRxBytes,
    long TrafficTxBytes,
    decimal VpnSpeedMbps,
    decimal InfraLatencyMs
) : IRequest<Unit>;