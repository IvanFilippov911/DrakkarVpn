using DrakkarVpn.Core.Api.Infrastructure.EF;
using MediatR;

namespace DrakkarVpn.Core.Pipeline;

public sealed class EfTransactionBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly AppDbContext _db;
    public EfTransactionBehavior(AppDbContext db) => _db = db;

    public async Task<TRes> Handle(
        TReq request, 
        RequestHandlerDelegate<TRes> next, 
        CancellationToken ct)
    {
        var isCommand = typeof(TReq).Namespace?.Contains(".Commands", StringComparison.OrdinalIgnoreCase) == true;
        if (!isCommand) return await next();
        
        if (_db.Database.CurrentTransaction is not null)
            return await next();

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var result = await next();    
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return result;
    }
}
