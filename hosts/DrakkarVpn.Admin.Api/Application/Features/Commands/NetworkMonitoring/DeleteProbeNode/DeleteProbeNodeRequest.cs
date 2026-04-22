using NetworkMonitoring.Application.Abstractions;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.DeleteProbeNode;

public sealed record DeleteProbeNodeRequest(Guid ProbeNodeId)
    : INetworkMonitoringCommand<bool>;

