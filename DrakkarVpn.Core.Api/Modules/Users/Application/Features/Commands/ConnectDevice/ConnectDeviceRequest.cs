using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ConnectDevice;

public sealed record ConnectDeviceRequest(
    string InitData,
    string? DeviceName,
    string? Platform,
    string? ExistingDeviceId
) : IRequest<ConnectDeviceResponse>;