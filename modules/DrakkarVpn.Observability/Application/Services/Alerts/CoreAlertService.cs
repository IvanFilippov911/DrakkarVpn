using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Services.Alerts;

public sealed class CoreAlertService : ICoreAlertService
{
    private readonly ICoreAlertRepository _repo;

    public CoreAlertService(ICoreAlertRepository repo) => _repo = repo;

    public async Task<Guid> CreateAsync(CreateCoreAlertArgs a, CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;

        var alert = new CoreAlert
        {
            Id           = Guid.NewGuid(),
            CreatedAtUtc = nowUtc,
            IsResolved   = false,

            Source   = a.Source,
            Code     = a.Code,
            Severity = a.Severity,
            Title    = a.Title,
            Message  = a.Message,

            ServerId    = a.ServerId,
            UserId      = a.UserId,
            Region      = a.Region,
            DetailsJson = a.DetailsJson
        };

        await _repo.AddAsync(alert, ct);
        return alert.Id;
    }

    public async Task CreateBatchAsync(IReadOnlyCollection<CreateCoreAlertArgs> items, CancellationToken ct)
    {
        if (items.Count == 0) return;

        var nowUtc = DateTime.UtcNow;

        var entities = items.Select(a => new CoreAlert
        {
            Id           = Guid.NewGuid(),
            CreatedAtUtc = nowUtc,
            IsResolved   = false,

            Source   = a.Source,
            Code     = a.Code,
            Severity = a.Severity,
            Title    = a.Title,
            Message  = a.Message,

            ServerId    = a.ServerId,
            UserId      = a.UserId,
            Region      = a.Region,
            DetailsJson = a.DetailsJson
        }).ToArray();

        await _repo.InsertManyAsync(entities, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _repo.DeleteAsync(id, ct);

    public Task<bool> ResolveAsync(ResolveCoreAlertArgs a, CancellationToken ct)
        => _repo.MarkResolvedAsync(a.Id, a.ResolutionType, a.ResolutionNote, a.ResolvedByAdminId, ct);
}