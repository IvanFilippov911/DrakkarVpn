namespace DrakkarVpn.AdminAuth.Application.Abstractions;

public interface IAdminAuthUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
