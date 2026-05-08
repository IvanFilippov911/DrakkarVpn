using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record AgentApplyServerTransportContext(
    Guid ServerId,
    Guid ActivationId,
    string AgentBaseUrl,
    string PublicHost,
    int PublicPort,
    string RealityPublicKey,
    TransportType TransportType,
    SecurityType SecurityType,
    string? RealitySni,
    string? RealityShortId,
    string? RealityFingerprint,
    string? RealityDest,
    string? GrpcServiceName,
    string? GrpcAuthority);
