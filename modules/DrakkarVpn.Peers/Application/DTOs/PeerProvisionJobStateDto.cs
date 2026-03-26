using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerProvisionJobStateDto(
    PeerProvisionState State,
    string? ErrorCode = null,
    string? ErrorMessage = null);