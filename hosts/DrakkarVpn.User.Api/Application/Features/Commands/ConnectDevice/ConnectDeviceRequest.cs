using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.ConnectDevice;

public sealed record ConnectDeviceRequest(
    string InitData,
    string? DeviceName,
    string? Platform,
    string? ExistingDeviceId
) : IRequest<ConnectDeviceResponse>, IPeersCommand<ConnectDeviceResponse>;