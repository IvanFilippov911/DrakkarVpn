using NetworkMonitoring.Application.Abstractions.Services;
using NetworkMonitoring.Application.DTOs;
using NetworkMonitoring.Application.Abstractions.Repositories;

namespace NetworkMonitoring.Application.Services;

public sealed class ProbeNodesQueryService : IProbeNodesQueryService
{
    private readonly IProbeNodeReadRepository _repo;

    public ProbeNodesQueryService(IProbeNodeReadRepository repo)
        => _repo = repo;

    public async Task<IReadOnlyList<ProbeNodeDto>> GetAllAsync(CancellationToken ct)
    {
        var items = await _repo.ListAsync(ct);

        return items.Select(x => new ProbeNodeDto(
            Id: x.Id,
            Name: x.Name,
            Region: x.Region,
            Host: x.Host,
            Status: x.Status,
            IsEnabled: x.IsEnabled,
            LastSeenAtUtc: x.LastSeenAtUtc,
            CreatedAtUtc: x.CreatedAtUtc,
            UpdatedAtUtc: x.UpdatedAtUtc
        )).ToList();
    }
}

