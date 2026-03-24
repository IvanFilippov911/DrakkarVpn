using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.StartVpnConfigProvisioning;

public sealed class StartVpnConfigProvisioningHandler
    : IRequestHandler<StartVpnConfigProvisioningRequest, StatusVpnConfigDto>
{
    private readonly IVpnAccessContextService _access;
    private readonly ICurrentVpnConfigService _current;
    private readonly IConfigProvisionService _provision;

    public StartVpnConfigProvisioningHandler(
        IVpnAccessContextService access,
        ICurrentVpnConfigService current,
        IConfigProvisionService provision)
    {
        _access = access;
        _current = current;
        _provision = provision;
    }

    public async Task<StatusVpnConfigDto> Handle(
        StartVpnConfigProvisioningRequest req,
        CancellationToken ct)
    {
        var access = await _access.GetVpnAccessContextAsync(req.TelegramId, req.DeviceId, ct);

        var config = await _current.GetCurrentConfigAsync(access, ct);
        if (config is not null)
            return StatusVpnConfigDto.Ready(config.ConfigRaw, config.HappLink);

        var jobId = await _provision.StartVpnConfigProvisioningAsync(access, ct);

        var pollUrl = $"/api/v1/user/vpn/config/provision/{jobId}";

        return StatusVpnConfigDto.Pending(jobId, pollUrl);
    }
}

