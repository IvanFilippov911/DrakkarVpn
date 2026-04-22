using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Application.DTOs;

public sealed record ProbeNodeDto(
    Guid Id,
    string Name,
    string Region,
    string Host,
    ProbeNodeStatus Status,
    bool IsEnabled,
    DateTime? LastSeenAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

