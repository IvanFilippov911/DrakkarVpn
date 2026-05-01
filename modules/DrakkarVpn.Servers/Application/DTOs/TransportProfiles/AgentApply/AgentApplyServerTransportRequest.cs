namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record AgentApplyServerTransportRequest(
    Guid OperationId,
    Guid ServerId,
    Guid ActivationId,
    string PublicHost,
    int PublicPort,
    string TransportType,
    string SecurityType,
    string RealitySni,
    string RealityShortId,
    string RealityFingerprint,
    string RealityPublicKey,
    string? RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority,
    string InboundTag,
    string Flow,
    string Encryption);