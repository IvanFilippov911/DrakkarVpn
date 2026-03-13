using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Abstracts.Services;

public interface ICoreErrorEventService
{
    Task LogAsync(LogCoreErrorEventArgs args, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}