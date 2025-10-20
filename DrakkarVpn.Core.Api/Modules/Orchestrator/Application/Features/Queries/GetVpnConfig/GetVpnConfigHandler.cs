using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;
using MediatR;
using Microsoft.Extensions.Options;

public sealed class GetVpnConfigHandler 
    : IRequestHandler<GetVpnConfigRequest, GetVpnConfigResponse?>
{
    private readonly IMediator _mediator;
    private readonly VpnLinkOptions _linkOptions;

    public GetVpnConfigHandler(IMediator mediator, IOptions<VpnLinkOptions> linkOptions)
    {
        _mediator = mediator;
        _linkOptions = linkOptions.Value;
    }

    public async Task<GetVpnConfigResponse?> Handle(GetVpnConfigRequest req, CancellationToken ct)
    {
        var user = await _mediator.Send(new GetUserByTelegramIdRequest(req.TelegramId), ct);
        if (user is null) return null;

        var peer = await _mediator.Send(new AllocatePeerRequest(
            TelegramId:     req.TelegramId,
            Region:         req.Region,
            DeviceId:       req.DeviceId,
            DeviceName:     req.DeviceName,
            Platform:       req.Platform
        ), ct);

        var happLink = _linkOptions.BuildHappLink(peer.AgentPeerId.ToString());
        
        return new GetVpnConfigResponse(
            peer.ConfigRaw,
            happLink
        );
    }
}