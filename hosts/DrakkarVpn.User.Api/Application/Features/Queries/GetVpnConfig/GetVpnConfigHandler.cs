using DrakkarVpn.Core.Api.Modules.Orchestrator.API;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Shared.Peers;
using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;
public sealed class GetVpnConfigHandler
    : IRequestHandler<GetVpnConfigRequest, GetVpnConfigResponse>
{
    private readonly IMediator _mediator;
    private readonly LinkGenerator _links;

    public GetVpnConfigHandler(IMediator mediator, LinkGenerator links)
    {
        _mediator = mediator;
        _links = links;
    }

    public async Task<GetVpnConfigResponse> Handle(GetVpnConfigRequest req, CancellationToken ct)
    {
        var alloc = await _mediator.Send(
            new AllocatePeerRequest(req.TelegramId, req.DeviceId),
            ct);

        if (alloc.Status == AllocatePeerStatus.Ready)
            return GetVpnConfigResponse.Ready(alloc.ConfigRaw!, alloc.HappLink!);

        var jobId = alloc.JobId ?? throw new InvalidOperationException("Accepted without JobId");

        var pollUrl = _links.GetPathByAction(
                action: nameof(UserFlowController.GetPeerProvisionJob),
                controller: "UserFlow",
                values: new { jobId }
            ) ?? $"/api/v1/user/vpn/provision/{jobId}";

        return GetVpnConfigResponse.Pending(jobId, pollUrl);
    }
}