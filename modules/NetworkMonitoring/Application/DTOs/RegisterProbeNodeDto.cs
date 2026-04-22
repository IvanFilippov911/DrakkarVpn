namespace NetworkMonitoring.Application.DTOs;

public sealed record RegisterProbeNodeDto(
    string Name,
    string Region,
    string Host);

