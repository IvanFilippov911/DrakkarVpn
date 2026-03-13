namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerMetricsHistoryService
{
    Task AppendFromStateAsync(
        IReadOnlyCollection<Guid> appliedServerIds,
        DateTime periodStartUtc,
        DateTime nowUtc,
        CancellationToken ct);
}