using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

namespace DrakkarVpn.Users.Infrastructure.EF;

public sealed class UsersUnitOfWork : IUsersUnitOfWork
{
    private readonly UsersDbContext _db;

    public UsersUnitOfWork(UsersDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}