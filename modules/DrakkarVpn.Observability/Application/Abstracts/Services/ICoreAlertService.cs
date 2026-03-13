using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Abstracts.Services;

public interface ICoreAlertService
{
    Task<Guid> CreateAsync(CreateCoreAlertArgs args, CancellationToken ct);
    Task CreateBatchAsync(IReadOnlyCollection<CreateCoreAlertArgs> items, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<bool> ResolveAsync(ResolveCoreAlertArgs args, CancellationToken ct);
}