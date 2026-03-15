using DrakkarVpn.AdminAuth.Application.Abstractions;

namespace DrakkarVpn.AdminAuth.Infrastructure.EF;

public sealed class AdminAuthUnitOfWork : IAdminAuthUnitOfWork
{
    private readonly AdminAuthDbContext _db;

    public AdminAuthUnitOfWork(AdminAuthDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
