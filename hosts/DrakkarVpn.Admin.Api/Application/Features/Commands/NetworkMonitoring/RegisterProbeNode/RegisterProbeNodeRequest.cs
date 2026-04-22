using NetworkMonitoring.Application.Abstractions;
using NetworkMonitoring.Application.DTOs;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.RegisterProbeNode;

public sealed record RegisterProbeNodeRequest(
    RegisterProbeNodeDto Data
) : INetworkMonitoringCommand<Guid>;

