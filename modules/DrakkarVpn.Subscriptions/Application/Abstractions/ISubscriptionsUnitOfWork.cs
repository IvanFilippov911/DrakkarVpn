namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Abstractions;

public interface ISubscriptionsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}