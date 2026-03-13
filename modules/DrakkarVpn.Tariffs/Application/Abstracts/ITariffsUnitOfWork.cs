namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;


public interface ITariffsUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}