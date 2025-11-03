using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerMetricsProcessor
{
    (IReadOnlyCollection<PeerMetricsHistoryItem> History,
        IReadOnlyCollection<PeerSyncIssueItem> SyncIssues)
        ApplyMetricsAndDetectIssues(
            Guid serverId,
            DateTime nowUtc,
            IReadOnlyCollection<Peer> peersOnServer,
            IReadOnlyCollection<PeerMetricsDto> items);
}