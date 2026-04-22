using NetworkMonitoring.Application.Abstractions;
using NetworkMonitoring.Application.DTOs;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.UpdateProbeNode;

public sealed record UpdateProbeNodeRequest(
    Guid ProbeNodeId,
    RegisterProbeNodeDto Data
) : INetworkMonitoringCommand<bool>;

