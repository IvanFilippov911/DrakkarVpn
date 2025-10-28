using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;

public sealed class RegisterPeerHandler : IRequestHandler<RegisterPeerRequest, PeerRegisterResponseDto>
{
    private readonly IPeerRepository _peers;
    private readonly IAppUserRepository _users;
    private readonly IServerRepository _servers;
    private readonly IPeersAgentClient _agent;

    public RegisterPeerHandler(
        IPeerRepository peers,
        IAppUserRepository users,
        IServerRepository servers,
        IPeersAgentClient agent)
    {
        _peers = peers;
        _users = users;
        _servers = servers;
        _agent = agent;
    }

    public async Task<PeerRegisterResponseDto> Handle(RegisterPeerRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct)
                   ?? throw new InvalidOperationException($"User {req.UserId} not found");

        var server = await _servers.GetAsync(req.ServerId, ct)
                    ?? throw new InvalidOperationException($"Server {req.ServerId} not found");

        if (!server.IsEligible())
            throw new InvalidOperationException($"Server {req.ServerId} is not eligible");

        AgentPeerUuid? agentPeerUuid = null;

        try
        {
            var agentResult = await _agent.RegisterPeerAsync(server, ct);
            agentPeerUuid = agentResult.PeerUuid;
            
            var peer = Peer.CreateNew(
                serverId:       req.ServerId,
                agentPeerUuid:  agentResult.PeerUuid,
                configRaw:      agentResult.ConfigRaw,
                deviceId:       req.DeviceId,
                nowUtc:         DateTime.UtcNow
            );

            await _peers.AddAsync(peer, ct);
            await _peers.SaveChangesAsync(ct);
            
            return new PeerRegisterResponseDto(peer.AgentPeerUuid.Value, peer.ConfigRaw);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg && pg.SqlState == "23505")
        {
            if (agentPeerUuid is not null)
            {
                try { await _agent.RevokePeerAsync(server, agentPeerUuid.Value, ct); } catch {  }
            }
            
            var existing = await _peers.GetByDeviceIdAsync(
                req.DeviceId, ct);

            if (existing is null) throw;

            return new PeerRegisterResponseDto(existing.AgentPeerUuid.Value, existing.ConfigRaw);
        }
        catch
        {
            if (agentPeerUuid is not null)
            {
                try { await _agent.RevokePeerAsync(server, agentPeerUuid.Value, ct); } catch { }
            }
            throw;
        }
    }
}
