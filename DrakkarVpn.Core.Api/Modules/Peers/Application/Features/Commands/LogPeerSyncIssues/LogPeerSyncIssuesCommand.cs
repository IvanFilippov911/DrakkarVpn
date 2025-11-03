using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands;

public sealed record LogPeerSyncIssuesCommand(
    Guid ServerId,
    IReadOnlyCollection<PeerSyncIssueItem> Items
) : IRequest<Unit>;