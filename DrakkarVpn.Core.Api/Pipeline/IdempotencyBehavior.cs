using System.Text.Json;
using DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Idempotency.Domain;
using MediatR;

namespace DrakkarVpn.Core.Pipeline;

public sealed class IdempotencyBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly IIdempotencyRepository _idem;

    public IdempotencyBehavior(IIdempotencyRepository idem) => _idem = idem;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (request is not IIdempotentRequest<TRes> idemReq)
            return await next(); 

        var key = await _idem.FindAsync(idemReq.ActorKey, idemReq.Action, idemReq.RequestId, ct);
        if (key is not null)
        {
            if (key.Status == IdempotencyStatus.Succeeded)
                return JsonSerializer.Deserialize<TRes>(key.ResultJson!)!;
            if (key.Status == IdempotencyStatus.Started)
                throw new InvalidOperationException("Duplicate request in progress");
        }

        key ??= await _idem.StartAsync(idemReq.ActorKey, idemReq.Action, idemReq.RequestId, TimeSpan.FromDays(30), ct);

        try
        {
            var result = await next();
            await _idem.MarkSucceededAsync(key.Id, JsonSerializer.Serialize(result), ct);
            return result;
        }
        catch (Exception ex)
        {
            await _idem.MarkFailedAsync(key.Id, ex.Message, ct);
            throw;
        }
    }
}

