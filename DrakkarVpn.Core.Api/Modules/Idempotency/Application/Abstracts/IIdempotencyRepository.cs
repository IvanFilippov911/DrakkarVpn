using DrakkarVpn.Core.Api.Modules.Idempotency.Domain;

namespace DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;

public interface IIdempotencyRepository
{
    Task<IdempotencyKey?> FindAsync(string actorKey, string action, Guid requestId, CancellationToken ct);
    Task<IdempotencyKey> StartAsync(string actorKey, string action, Guid requestId, TimeSpan? ttl, CancellationToken ct);
    Task MarkSucceededAsync(Guid id, string? resultJson, CancellationToken ct);
    Task MarkFailedAsync(Guid id, string error, CancellationToken ct);
}
