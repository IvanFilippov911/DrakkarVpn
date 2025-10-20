using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersBySubscription;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed class AllocatePeerHandler : IRequestHandler<AllocatePeerRequest, PeerRegisterResponseDto>
{
    private readonly IMediator _mediator;
    public AllocatePeerHandler(IMediator mediator) => _mediator = mediator;

    public async Task<PeerRegisterResponseDto> Handle(AllocatePeerRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            throw new ArgumentException("DeviceId is required", nameof(req.DeviceId));
        
        var user = await _mediator.Send(new GetUserByTelegramIdRequest(req.TelegramId), ct);
        
        var sub = await _mediator.Send(new GetActiveSubscriptionByUserRequest(user.Id), ct);
        if (sub is null)
            throw new InvalidOperationException("No active subscription");
        
        var peers = await _mediator.Send(new GetPeersBySubscriptionRequest(sub.Id), ct);
        var existing = peers.FirstOrDefault(p =>
            p.Status == PeerStatus.Active &&
            string.Equals(p.DeviceId, req.DeviceId, StringComparison.Ordinal));

        if (existing is not null)
            return new PeerRegisterResponseDto(existing.AgentPeerId, existing.ConfigRaw);
        
        var used = peers.Count(p => p.Status == PeerStatus.Active);
        if (used >= sub.MaxDevices)
            throw new InvalidOperationException("Device limit reached");
        
        var servers = await _mediator.Send(new GetServersRequest(req.Region, nameof(ServerStatus.Enabled)), ct);
        var server = servers
            .OrderBy(s => s.PeersActive)
            .FirstOrDefault();
        if (server is null)
            throw new InvalidOperationException("No enabled servers available");
        
        var createReq = new RegisterPeerRequest(
            user.Id,
            server.Id,
            sub.Id,
            req.DeviceId,
            req.DeviceName,
            req.Platform
        );
      
        var peer = await _mediator.Send(createReq, ct);

        return peer;
    }
}