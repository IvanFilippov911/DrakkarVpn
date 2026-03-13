using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Options;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared.Errors;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed class AllocatePeerHandler : IRequestHandler<AllocatePeerRequest, AllocatePeerDto>
{
    private readonly IMediator _mediator;
    private readonly IPeerProvisionJobsService _jobs;
    private readonly IPeerProvisionPayloadFactory _payloadFactory;
    private readonly VpnLinkOptions _link;
    private readonly ISubscriptionQueryService _subsQuery;
    private readonly IUsersQueryService _usersQuery;
    private readonly IServersQueryService _serversQuery;
    private readonly IPeersQueryService _peerQuery;

    public AllocatePeerHandler(
        IMediator mediator,
        IPeerProvisionJobsService jobs,
        IPeerProvisionPayloadFactory payloadFactory,
        IOptions<VpnLinkOptions> linkOptions,
        ISubscriptionQueryService subsQuery,
        IUsersQueryService usersQuery,
        IServersQueryService serversQuery,
        IPeersQueryService peerQuery)
    {
        _mediator        = mediator;
        _jobs            = jobs;
        _payloadFactory  = payloadFactory;
        _link            = linkOptions.Value;
        _subsQuery       = subsQuery;
        _usersQuery      = usersQuery;
        _serversQuery     = serversQuery;
        _peerQuery       = peerQuery;
    }

    public async Task<AllocatePeerDto> Handle(AllocatePeerRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DeviceId))
            throw new ArgumentException("DeviceId is required", nameof(req.DeviceId));

        var user = await _usersQuery.GetByTelegramIdAsync(req.TelegramId, ct)
                   ?? throw new InvalidOperationException("User not found");

        if (user.Status == UserStatus.Banned)
            throw new UserBannedException(user.Id);

        var nowUtc = DateTime.UtcNow;

        var sub = await _subsQuery.GetActiveByUserAsync(user.Id, nowUtc, ct);
        if (sub is null)
            throw new InvalidOperationException("No active subscription");

        var existingPeer = await _peerQuery.GetPeerByDeviceAsync(req.DeviceId, ct);
        if (existingPeer is not null)
        {
            var happ = _link.BuildHappLink(existingPeer.AgentPeerId.ToString());
            return AllocatePeerDto.Ready(existingPeer.AgentPeerId, existingPeer.ConfigRaw, happ);
        }

        var servers = await _serversQuery.GetListAsync(nameof(ServerStatus.Enabled), ct);
        var server = servers.OrderBy(s => s.PeersActive).FirstOrDefault()
                     ?? throw new InvalidOperationException("No enabled servers available");
        
        var payload = _payloadFactory.CreateNew();

        var jobId = await _jobs.EnqueueAsync(
            new PeerProvisionJobCreateDto(
                UserId: user.Id,
                DeviceId: req.DeviceId,
                ServerId: server.Id,
                AgentPeerUuid: payload.AgentPeerUuid,
                ConfigRaw: payload.ConfigRaw,
                MaxAttempts: 10),
            nowUtc,
            ct);

        return AllocatePeerDto.Accepted(jobId);
    }
}