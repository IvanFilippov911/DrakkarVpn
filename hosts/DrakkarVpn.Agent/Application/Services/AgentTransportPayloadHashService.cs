using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class AgentTransportPayloadHashService : IAgentTransportPayloadHashService
{
    public string Calculate(ApplyServerTransportRequestDto request)
    {
        var canonical = BuildCanonicalString(request);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string BuildCanonicalString(ApplyServerTransportRequestDto request)
    {
        var lines = new[]
        {
            $"encryption={Norm(request.Encryption)}",
            $"flow={Norm(request.Flow)}",
            $"grpc_authority={Norm(request.GrpcAuthority)}",
            $"grpc_service_name={Norm(request.GrpcServiceName)}",
            $"inbound_tag={Norm(request.InboundTag)}",
            $"public_host={Norm(request.PublicHost)}",
            $"public_port={request.PublicPort.ToString(CultureInfo.InvariantCulture)}",
            $"reality_dest={Norm(request.RealityDest)}",
            $"reality_fingerprint={Norm(request.RealityFingerprint)}",
            $"reality_public_key={Norm(request.RealityPublicKey)}",
            $"reality_short_id={Norm(request.RealityShortId)}",
            $"reality_sni={Norm(request.RealitySni)}",
            $"security_type={Norm(request.SecurityType)}",
            $"transport_type={Norm(request.TransportType)}"
        };

        return string.Join('\n', lines);
    }

    private static string Norm(string? s) =>
        string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim().ToLowerInvariant();
}
