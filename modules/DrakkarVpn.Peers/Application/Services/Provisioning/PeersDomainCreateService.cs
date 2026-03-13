using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Validation;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeersDomainCreateService : IPeersDomainCreateService
{
    private readonly IPeerRepository _peerRepo;
    private readonly ILogger<PeersDomainCreateService> _log;

    public PeersDomainCreateService(
        IPeerRepository peerRepo,
        ILogger<PeersDomainCreateService> log)
    {
        _peerRepo = peerRepo ?? throw new ArgumentNullException(nameof(peerRepo));
        _log      = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<IReadOnlyList<PeerCreationResult>> CreateBatchAsync(
        IReadOnlyCollection<PeerBatchCreateDto> dtos,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (dtos is null || dtos.Count == 0)
            return Array.Empty<PeerCreationResult>();

        nowUtc = EnsureUtc(nowUtc);
        
        var map = PeerBatchNormalizer.NormalizeAndDedupe(dtos);
        if (map.Count == 0)
            return Array.Empty<PeerCreationResult>();

        var deviceIds = map.Keys.ToArray();
        
        var existing1 = await _peerRepo.GetPeerIdsByDeviceIdsAsync(deviceIds, ct);
        
        var toInsert = new List<Peer>(capacity: Math.Max(0, map.Count - existing1.Count));

        foreach (var dto in map.Values)
        {
            if (existing1.ContainsKey(dto.DeviceId))
                continue;

            toInsert.Add(Peer.CreateNew(
                serverId: dto.ServerId,
                agentPeerUuid: dto.AgentPeerUuid,
                configRaw: dto.ConfigRaw!,
                deviceId: dto.DeviceId,
                nowUtc: nowUtc));
        }
        
        if (toInsert.Count > 0)
        {
            try
            {
                await _peerRepo.CreateManyIgnoreConflictsAsync(toInsert, ct);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "CreateManyIgnoreConflictsAsync failed for {Count} peers", toInsert.Count);
            }
        }
        
        var existing2 = await _peerRepo.GetPeerIdsByDeviceIdsAsync(deviceIds, ct);
        
        var results = new List<PeerCreationResult>(map.Count);
        foreach (var dto in map.Values)
        {
            if (existing2.TryGetValue(dto.DeviceId, out var peerId))
            {
                results.Add(new PeerCreationResult(
                    JobId: dto.JobId,
                    Success: true,
                    PeerId: peerId));
                continue;
            }

            results.Add(new PeerCreationResult(
                JobId: dto.JobId,
                Success: false,
                PeerId: null,
                ErrorCode: "PeerNotFoundAfterUpsert",
                ErrorMessage: null));
        }

        return results;
    }

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}