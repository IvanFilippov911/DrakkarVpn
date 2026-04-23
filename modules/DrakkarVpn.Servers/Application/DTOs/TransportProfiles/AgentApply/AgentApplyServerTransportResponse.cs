namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record AgentApplyServerTransportResponse(
    bool Applied,
    string? ErrorCode,
    string? ErrorMessage);