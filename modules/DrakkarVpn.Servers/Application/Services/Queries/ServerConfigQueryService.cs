using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Shared.Servers;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Servers.Application.Services.Queries;

public sealed class ServerConfigQueryService : IServerConfigQueryService
{
    private readonly IServerRepository _servers;
    private readonly ITransportProfileWriteRepository _transportProfiles;

    public ServerConfigQueryService(
        IServerRepository servers,
        ITransportProfileWriteRepository transportProfiles)
    {
        _servers = servers;
        _transportProfiles = transportProfiles;
    }

    public async Task<ServerConfigDataDto?> GetDataForConfigByIdAsync(
        Guid serverId,
        CancellationToken ct)
    {
        var server = await _servers.GetAsync(serverId, ct);
        if (server is null)
            return null;

        var activeActivation = server.TransportActivations
            .FirstOrDefault(a => a.Status == TransportActivationStatus.Active);
        if (activeActivation is null)
            return null;

        var profile = await _transportProfiles.GetAsync(activeActivation.TransportProfileId, ct);
        if (profile is null)
            return null;

        return new ServerConfigDataDto(
            Region: server.Region.Code,
            PublicHost: server.PublicHost.Value,
            PublicPort: server.PublicPort,
            TransportType: profile.TransportType,
            SecurityType: profile.SecurityType,
            RealitySni: profile.RealitySni!,
            RealityPublicKey: activeActivation.RealityPublicKey,
            RealityShortId: profile.RealityShortId!,
            RealityFingerprint: profile.RealityFingerprint!,
            RealityDest: profile.RealityDest,
            GrpcServiceName: profile.GrpcServiceName,
            GrpcAuthority: profile.GrpcAuthority);
    }

    public Task<Guid[]> GetEnabledServerIdsAsync(CancellationToken ct)
        => _servers.Query()
            .AsNoTracking()
            .Where(s => s.Status == ServerStatus.Enabled)
            .Select(s => s.Id)
            .ToArrayAsync(ct);
}
