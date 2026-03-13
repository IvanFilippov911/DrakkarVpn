namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerTrafficWindowRow(
    Guid PeerId,
    long TrafficBytes
);