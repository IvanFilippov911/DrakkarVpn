using System.Collections.Concurrent;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Shared.Errors;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerRevocationService : IPeerRevocationService
{
    private readonly IPeerRepository     _peers;
    private readonly IPeersAgentClient _agentClient;
    private readonly IServerQueryForPeers _serversQuery;

    private const int MaxParallel = 10;

    public PeerRevocationService(
        IPeerRepository peers,
        IPeersAgentClient agentClient,
        IServerQueryForPeers  serversQuery)
    {
        _peers        = peers;
        _agentClient = agentClient;
        _serversQuery = serversQuery;
    }
    
    public async Task<bool> RevokePeerAsync(Guid serverId, Guid peerId, CancellationToken ct)
    {
        var peer = await _peers.GetByIdAsync(peerId, ct);
        if (peer is null) return false;

        if (!peer.IsActive()) return true;

        if (peer.ServerId != serverId)
            throw new InvalidOperationException(
                $"Peer {peer.Id} belongs to server {peer.ServerId}, not {serverId}");
        
        var baseUrl = await _serversQuery.GetAgentBaseUrlAsync(serverId, ct);
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("AgentBaseUrl not resolved");
        
        var ok = await _agentClient.RevokePeerAsync(baseUrl, peer.AgentPeerUuid, ct);
        if (!ok)
            throw new InvalidOperationException($"Agent {serverId} refused to revoke peer {peer.Id}");

        var markerUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        await _peers.MarkRevokedAsync(peer.Id, markerUtc, ct);
        
        var okMarker = await _peers.IsRevokedByMarkerAsync(peer.Id, markerUtc, ct);
        if (!okMarker)
            throw new InvalidOperationException($"Peer revoke marker mismatch for {peer.Id}");

        return true;
    }
    
    
    public async Task<BulkPeersRevokeResultDto> RevokeUsersPeersAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();
        if (ids.Length == 0)
            return new BulkPeersRevokeResultDto([], [], []);
        
        var peersForDelete = await _peers.GetActiveForRevokeByUsersAsync(ids, ct);
        if (peersForDelete.Count == 0)
            return new BulkPeersRevokeResultDto(ids.ToList(), [], []);

        var serverIds = peersForDelete.Select(x => x.ServerId).Distinct().ToArray();
        var baseUrls  = await _serversQuery.GetAgentBaseUrlsByIdsAsync(serverIds, ct);

        using var sem = new SemaphoreSlim(MaxParallel);
        var failures      = new ConcurrentBag<BulkPeerRevokeFailure>();
        var revokedPeerIds = new ConcurrentBag<Guid>();

        var tasks = peersForDelete.Select(async row =>
        {
            await sem.WaitAsync(ct);
            try
            {
                if (!baseUrls.TryGetValue(row.ServerId, out var baseUrl))
                {
                    failures.Add(new BulkPeerRevokeFailure(
                        row.UserId, row.PeerId, row.ServerId, "AgentBaseUrlNotResolved"));
                    return;
                }

                var ok = await _agentClient.RevokePeerAsync(baseUrl, row.AgentPeerUuid, ct);
                
                if (!ok)
                {
                    failures.Add(new BulkPeerRevokeFailure(
                        row.UserId, row.PeerId, row.ServerId, "AgentRevokeFailed"));
                    return;
                }

                revokedPeerIds.Add(row.PeerId);
            }
            catch (Exception ex)
            {
                failures.Add(new BulkPeerRevokeFailure(
                    row.UserId, row.PeerId, row.ServerId, "UnhandledException", ex.Message));
            }
            finally
            {
                sem.Release();
            }
        });

        await Task.WhenAll(tasks);

        var revokedPeers = revokedPeerIds.Distinct().ToList();
        if (revokedPeers.Count == 0)
        {
            var failedUsers0 = failures.Select(x => x.UserId).Distinct().ToList();
            return new BulkPeersRevokeResultDto([], failedUsers0, failures.ToList());
        }
        
        await _peers.RevokedManyAsync(revokedPeers, markerUtc, ct);
        
        var revokedByMarker = await _peers.GetRevokedPeerIdsByMarkerAsync(revokedPeers, markerUtc, ct);
        var revokedSet      = revokedByMarker.ToHashSet();
        var attemptedSet    = revokedPeers.ToHashSet();

        foreach (var row in peersForDelete.Where(x => attemptedSet.Contains(x.PeerId)))
        {
            if (!revokedSet.Contains(row.PeerId))
            {
                failures.Add(new BulkPeerRevokeFailure(
                    row.UserId, row.PeerId, row.ServerId, "PeerRevokeMarkerMismatch"));
            }
        }

        var failedUsers = failures.Select(x => x.UserId).Distinct().ToHashSet();

        var succeededUsers = ids
            .Where(u => !failedUsers.Contains(u))
            .ToList();

        return new BulkPeersRevokeResultDto(
            SucceededUserIds: succeededUsers,
            FailedUserIds: failedUsers.ToList(),
            FailureDetails: failures.ToList());
    }
    
    
    public async Task<int> RevokeAllServerPeersAsync(Guid serverId, CancellationToken ct)
    {
        var peers = await _peers.GetListActiveByServerAsync(serverId, ct);
        if (peers.Count == 0)
            return 0;

        var markerUtc = DateTime.UtcNow;

        using var sem = new SemaphoreSlim(MaxParallel);
        var revokedIds = new ConcurrentBag<Guid>();
        var failed = new ConcurrentBag<Guid>();
        
        var baseUrl = await _serversQuery.GetAgentBaseUrlAsync(serverId, ct);
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("AgentBaseUrl not resolved");

        var tasks = peers.Select(async peer =>
        {
            await sem.WaitAsync(ct);
            try
            {
                var ok = await _agentClient.RevokePeerAsync(baseUrl, peer.AgentPeerUuid, ct);
                if (ok) revokedIds.Add(peer.Id);
                else    failed.Add(peer.Id);
            }
            finally
            {
                sem.Release();
            }
        });

        await Task.WhenAll(tasks);

        var revoked = revokedIds.Distinct().ToList();
        var failedCount = failed.Distinct().Count();

        if (revoked.Count > 0)
            await _peers.RevokedManyAsync(revoked, markerUtc, ct);

        if (failedCount > 0)
            throw PeersRevokeFailedException.ForServer(serverId, revoked.Count, failedCount);

        return revoked.Count;
    }
}