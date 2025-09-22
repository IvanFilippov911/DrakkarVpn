using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;

public sealed class RegisterPeerHandler: IRequestHandler<RegisterPeerRequest, PeerRegisterResponseDto>
{
    private readonly IPeerRepository _peers;
    private readonly IAppUserRepository _users;
    private readonly IServerRepository _servers;
    private readonly IAgentClient _agentClient;

    public RegisterPeerHandler(
        IPeerRepository peers,
        IAppUserRepository users,
        IServerRepository servers,
        IAgentClient agentClient)
    {
        _peers = peers;
        _users = users;
        _servers = servers;
        _agentClient = agentClient;
    }

    public async Task<PeerRegisterResponseDto> Handle(RegisterPeerRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null)
            throw new InvalidOperationException($"User {req.UserId} not found");
        
        var server = await _servers.GetAsync(new ServerId(req.ServerId), ct);
        if (server is null)
            throw new InvalidOperationException($"Server {req.ServerId} not found");

        if (!server.IsEligible())
            throw new InvalidOperationException($"Server {req.ServerId} is not eligible");
        
        AgentPeerUuid? createdPeerUuid = null;

        try
        {
            var agentResult = await _agentClient.RegisterPeerAsync(server, ct);
            createdPeerUuid = agentResult.PeerUuid;
            
            var peer = Peer.CreateNew(
                req.UserId,
                req.ServerId,
                agentResult.PeerUuid,
                agentResult.ConfigRaw,
                DateTime.UtcNow,
                req.ExpiresAt
            );
            
            await _peers.AddAsync(peer, ct);
            
            return new PeerRegisterResponseDto(
                peer.Id.Value,
                peer.ServerId,
                peer.ConfigRaw,
                peer.CreatedAt,
                peer.ExpiresAt
            );
        }
        catch
        {
            if (createdPeerUuid is not null)
            {
                await _agentClient.RevokePeerAsync(server, createdPeerUuid.Value, ct);
            }

            throw; 
        }
    }
}