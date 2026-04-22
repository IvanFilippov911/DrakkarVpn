using NetworkMonitoring.Domain;

namespace DrakkarVpn.Admin.Api.API.Contracts.NetworkMonitoring;

public sealed record ProbeNodeApiResponse(
    Guid Id,
    string Name,
    string Region,
    string Host,
    ProbeNodeStatus Status,
    bool IsEnabled,
    DateTime? LastSeenAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

