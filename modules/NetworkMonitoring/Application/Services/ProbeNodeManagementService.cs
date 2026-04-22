using DrakkarVpn.Shared.Errors.DomainErrors;
using NetworkMonitoring.Application.Abstractions.Repositories;
using NetworkMonitoring.Application.Abstractions.Services;
using NetworkMonitoring.Application.DTOs;
using NetworkMonitoring.Application.Errors;
using NetworkMonitoring.Domain;

namespace NetworkMonitoring.Application.Services;

public sealed class ProbeNodeManagementService : IProbeNodeManagementService
{
    private readonly IProbeNodeWriteRepository _repo;

    public ProbeNodeManagementService(IProbeNodeWriteRepository repo)
        => _repo = repo;

    public async Task<bool> UpdateAsync(
        Guid probeNodeId,
        RegisterProbeNodeDto dto,
        CancellationToken ct)
    {
        dto = dto ?? throw new ArgumentNullException(nameof(dto));

        var entity = await _repo.GetAsync(probeNodeId, ct);
        if (entity is null)
            return false;

        var name = dto.Name ?? throw new ArgumentNullException(nameof(dto.Name));
        var region = dto.Region ?? throw new ArgumentNullException(nameof(dto.Region));
        var host = dto.Host ?? throw new ArgumentNullException(nameof(dto.Host));

        var normalizedName = name.Trim();
        var normalizedHost = host.Trim();

        if (!string.Equals(normalizedName, entity.Name, StringComparison.Ordinal) &&
            await _repo.ExistsByNameAsync(normalizedName, ct))
            throw new DomainException(
                DomainArea.NetworkMonitoring,
                NetworkMonitoringErrorCodes.ProbeNodeNameAlreadyExists,
                $"Probe node with name '{normalizedName}' already exists.");

        if (!string.Equals(normalizedHost, entity.Host, StringComparison.Ordinal) &&
            await _repo.ExistsByHostAsync(normalizedHost, ct))
            throw new DomainException(
                DomainArea.NetworkMonitoring,
                NetworkMonitoringErrorCodes.ProbeNodeHostAlreadyExists,
                $"Probe node with host '{normalizedHost}' already exists.");

        var utcNow = DateTime.UtcNow;
        entity.Rename(name, utcNow);
        entity.ChangeRegion(region, utcNow);
        entity.ChangeHost(host, utcNow);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid probeNodeId, CancellationToken ct)
    {
        var entity = await _repo.GetAsync(probeNodeId, ct);
        if (entity is null)
            return false;

        await _repo.DeleteAsync(entity, ct);
        
        return true;
    }
}

