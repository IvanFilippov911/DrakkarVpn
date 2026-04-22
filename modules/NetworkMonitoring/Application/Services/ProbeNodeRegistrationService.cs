using DrakkarVpn.Shared.Errors.DomainErrors;
using NetworkMonitoring.Application.Abstractions.Services;
using NetworkMonitoring.Application.DTOs;
using NetworkMonitoring.Application.Errors;
using NetworkMonitoring.Domain;
using NetworkMonitoring.Application.Abstractions.Repositories;

namespace NetworkMonitoring.Application.Services;

public sealed class ProbeNodeRegistrationService : IProbeNodeRegistrationService
{
    private readonly IProbeNodeWriteRepository _repo;

    public ProbeNodeRegistrationService(IProbeNodeWriteRepository repo)
        => _repo = repo;

    public async Task<Guid> RegisterAsync(RegisterProbeNodeDto dto, CancellationToken ct)
    {
        dto = dto ?? throw new ArgumentNullException(nameof(dto));

        var name = dto.Name ?? throw new ArgumentNullException(nameof(dto.Name));
        var region = dto.Region ?? throw new ArgumentNullException(nameof(dto.Region));
        var host = dto.Host ?? throw new ArgumentNullException(nameof(dto.Host));
        
        var normalizedName = name.Trim();
        var normalizedHost = host.Trim();

        if (await _repo.ExistsByNameAsync(normalizedName, ct))
            throw new DomainException(
                DomainArea.NetworkMonitoring,
                NetworkMonitoringErrorCodes.ProbeNodeNameAlreadyExists,
                $"Probe node with name '{normalizedName}' already exists.");

        if (await _repo.ExistsByHostAsync(normalizedHost, ct))
            throw new DomainException(
                DomainArea.NetworkMonitoring,
                NetworkMonitoringErrorCodes.ProbeNodeHostAlreadyExists,
                $"Probe node with host '{normalizedHost}' already exists.");

        var utcNow = DateTime.UtcNow;
        var entity = ProbeNode.Register(
            Guid.NewGuid(),
            name,
            region,
            host,
            utcNow);

        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }
}

