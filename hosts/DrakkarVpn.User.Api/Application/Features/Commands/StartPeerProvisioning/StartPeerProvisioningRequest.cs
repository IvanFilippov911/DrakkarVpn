using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.StartVpnConfigProvisioning;

public sealed record StartPeerProvisioningRequest(
    long TelegramId,
    string DeviceId
) : IUsersCommand<StatusVpnConfigDto>;

