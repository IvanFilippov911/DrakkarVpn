using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;

public sealed class AppUserRepository : IAppUserRepository
{
    private readonly AppDbContext _db;
    public AppUserRepository(AppDbContext db) => _db = db;

    public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<AppUser?> GetByTelegramIdAsync(TelegramId tgId, CancellationToken ct) =>
        _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.TelegramId == tgId, ct);

    public async Task AddAsync(AppUser user, CancellationToken ct) =>
        await _db.Users.AddAsync(user, ct);
    
    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct) =>
        await _db.Users.AsNoTracking().ToListAsync(ct);
    
}