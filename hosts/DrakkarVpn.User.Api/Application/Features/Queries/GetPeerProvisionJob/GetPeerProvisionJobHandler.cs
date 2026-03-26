using DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerProvisionJob;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Shared.Peers;
using MediatR;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerProvisionJob;

public sealed class GetPeerProvisionJobHandler
    : IRequestHandler<GetPeerProvisionJobQuery, PeerProvisionJobStatusDto?>
{
    private readonly IPeerProvisionJobsService _svc;
    private readonly VpnLinkOptions _link;

    public GetPeerProvisionJobHandler(
        IPeerProvisionJobsService svc,
        IOptions<VpnLinkOptions> linkOptions)
    {
        _svc = svc;
        _link = linkOptions.Value;
    }

    public async Task<PeerProvisionJobStatusDto?> Handle(GetPeerProvisionJobQuery q, CancellationToken ct)
    {
        var job = await _svc.GetAsync(q.JobId, ct);
        if (job is null) return null;

        var status = job.State switch
        {
            PeerProvisionState.Ready  => PeerProvisionStatus.Ready,
            PeerProvisionState.Failed => PeerProvisionStatus.Failed,
            _                         => PeerProvisionStatus.Pending
        };

        return new PeerProvisionJobStatusDto(
            Status: status,
            ErrorCode: job.ErrorCode,
            ErrorMessage: job.ErrorMessage
        );
        
    }
}