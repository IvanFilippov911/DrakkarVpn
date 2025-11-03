using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerMetricsProcessor : IPeerMetricsProcessor
{
    public (IReadOnlyCollection<PeerMetricsHistoryItem> History,
            IReadOnlyCollection<PeerSyncIssueItem> SyncIssues)
            ApplyMetricsAndDetectIssues(
            Guid serverId,
            DateTime nowUtc,
            IReadOnlyCollection<Peer> peersOnServer,
            IReadOnlyCollection<PeerMetricsDto> items)
    {
        if (items.Count == 0 || peersOnServer.Count == 0)
            return (Array.Empty<PeerMetricsHistoryItem>(), Array.Empty<PeerSyncIssueItem>());

        var peersByAgentUuid    = BuildPeersMap(peersOnServer);
        var agentUuidsFromAgent = BuildAgentUuidSet(items);

        var history    = new List<PeerMetricsHistoryItem>(items.Count);
        var syncIssues = new List<PeerSyncIssueItem>();
        
        var knownItems = DetectUnknownPeersOnCore(
            serverId,
            nowUtc,
            items,
            peersByAgentUuid,
            syncIssues);
        
        ProcessKnownMetrics(
            nowUtc,
            knownItems,
            peersByAgentUuid,
            history);
        
        DetectMissingPeersOnAgent(
            serverId,
            nowUtc,
            peersOnServer,
            agentUuidsFromAgent,
            syncIssues);

        return (history, syncIssues);
    }

    private static Dictionary<Guid, Peer> BuildPeersMap(IReadOnlyCollection<Peer> peersOnServer)
    {
        return peersOnServer
            .Where(p => p.AgentPeerUuid != Guid.Empty)
            .ToDictionary(p => p.AgentPeerUuid, p => p);
    }

    private static HashSet<Guid> BuildAgentUuidSet(IReadOnlyCollection<PeerMetricsDto> items)
    {
        return items
            .Select(i => i.AgentPeerId)
            .Where(id => id != Guid.Empty)
            .ToHashSet();
    }
    
    
    
    private static void ProcessKnownMetrics(
        DateTime nowUtc,
        IReadOnlyCollection<PeerMetricsDto> items,
        IReadOnlyDictionary<Guid, Peer> peersByAgentUuid,
        ICollection<PeerMetricsHistoryItem> history)
    {
        foreach (var dto in items)
        {
            var peer = peersByAgentUuid[dto.AgentPeerId];

            if (dto.RxBytesTotal is not null || dto.TxBytesTotal is not null)
            {
                var rxAgent = dto.RxBytesTotal ?? peer.TotalRxBytes;
                var txAgent = dto.TxBytesTotal ?? peer.TotalTxBytes;

                var rxDelta = Math.Max(0, rxAgent - peer.TotalRxBytes);
                var txDelta = Math.Max(0, txAgent - peer.TotalTxBytes);

                peer.AddTraffic(rxDelta, txDelta, nowUtc);
            }

            if (dto.VpnLatencyMs is not null)
                peer.UpdateVpnLatencyMs(dto.VpnLatencyMs.Value, nowUtc);

            peer.RefreshOnlineStatus(nowUtc);

            history.Add(new PeerMetricsHistoryItem(
                PeerId:          peer.Id,
                TotalRxBytes:    peer.TotalRxBytes,
                TotalTxBytes:    peer.TotalTxBytes,
                IsOnline:        peer.IsOnline,
                VpnLatencyMs:    peer.VpnLatencyMs,
                LastDataAt:      peer.LastDataAt,
                LastLatencyAt:   peer.LastLatencyAt
            ));
        }
    }
    
    
    private static IReadOnlyCollection<PeerMetricsDto> DetectUnknownPeersOnCore(
        Guid serverId,
        DateTime nowUtc,
        IReadOnlyCollection<PeerMetricsDto> items,
        IReadOnlyDictionary<Guid, Peer> peersByAgentUuid,
        ICollection<PeerSyncIssueItem> syncIssues)
    {
        var known = new List<PeerMetricsDto>(items.Count);

        foreach (var dto in items)
        {
            if (!peersByAgentUuid.ContainsKey(dto.AgentPeerId))
            {
                syncIssues.Add(new PeerSyncIssueItem(
                    ServerId:      serverId,
                    PeerId:        null,
                    AgentPeerId:   dto.AgentPeerId,
                    Type:          PeerSyncIssueType.AgentHasUnknownPeer,
                    DetectedAtUtc: nowUtc,
                    Details:       "Metrics received for unknown AgentPeerUuid"
                ));

                continue;
            }

            known.Add(dto);
        }

        return known;
    }

    private static void DetectMissingPeersOnAgent(
        Guid serverId,
        DateTime nowUtc,
        IReadOnlyCollection<Peer> peersOnServer,
        IReadOnlySet<Guid> agentUuidsFromAgent,
        ICollection<PeerSyncIssueItem> syncIssues)
    {
        foreach (var peer in peersOnServer)
        {
            if (peer.AgentPeerUuid == Guid.Empty)
                continue;

            if (agentUuidsFromAgent.Contains(peer.AgentPeerUuid))
                continue;

            syncIssues.Add(new PeerSyncIssueItem(
                ServerId:      serverId,
                PeerId:        peer.Id,
                AgentPeerId:   peer.AgentPeerUuid,
                Type:          PeerSyncIssueType.CoreHasPeerMissingOnAgent,
                DetectedAtUtc: nowUtc,
                Details:       "Peer exists in Core but not present in agent metrics batch"
            ));
        }
    }
}