

using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.ConnectDevice;

public sealed record ConnectDeviceRequest(
    string InitData,
    string? DeviceName,
    string? Platform,
    string? ExistingDeviceId
) : IPeersCommand<ConnectDeviceResultDto>;