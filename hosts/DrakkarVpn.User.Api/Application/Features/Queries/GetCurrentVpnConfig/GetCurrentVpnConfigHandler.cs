using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetCurrentVpnConfig;

public sealed class GetCurrentVpnConfigHandler
    : IRequestHandler<GetCurrentVpnConfigRequest, StatusVpnConfigDto>
{
    private readonly IVpnAccessContextService _access;
    private readonly ICurrentVpnConfigService _current;
    private readonly IPeerProvisionJobsService _jobs;

    public GetCurrentVpnConfigHandler(
        IVpnAccessContextService access,
        ICurrentVpnConfigService current,
        IPeerProvisionJobsService jobs)
    {
        _access = access;
        _current = current;
        _jobs = jobs;
    }

    public async Task<StatusVpnConfigDto> Handle(GetCurrentVpnConfigRequest req, CancellationToken ct)
    {
        var access = await _access.GetVpnAccessContextAsync(req.TelegramId, req.DeviceId, ct);

        var jobId = await _jobs.GetActiveJobIdByDeviceIdAsync(access.DeviceId, ct);
        if (jobId is not null)
        {
            var pollUrl = $"/api/v1/user/vpn/config/provision/{jobId}";

            return StatusVpnConfigDto.Pending(jobId.Value, pollUrl);
        }

        var config = await _current.GetCurrentConfigAsync(access, ct);
        if (config is not null)
            return StatusVpnConfigDto.Ready(config.ConfigRaw, config.HappLink, config.V2rayLink);

        return StatusVpnConfigDto.NotStarted();
    }
}

