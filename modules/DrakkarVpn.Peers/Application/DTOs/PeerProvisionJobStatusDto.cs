
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

public sealed record PeerProvisionJobStatusDto(
    PeerProvisionStatus Status,
    string? ErrorCode = null,
    string? ErrorMessage = null);