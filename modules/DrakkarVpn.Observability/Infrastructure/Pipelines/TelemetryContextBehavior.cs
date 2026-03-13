using System.Diagnostics;
using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using DrakkarVpn.Observability.Application.Telemetry;
using MediatR;

namespace DrakkarVpn.Observability.Infrastructure.Pipelines;

public sealed class TelemetryContextBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly IRequestTelemetryContextAccessor _accessor;

    public TelemetryContextBehavior(IRequestTelemetryContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public async Task<TRes> Handle(
        TReq request,
        RequestHandlerDelegate<TRes> next,
        CancellationToken ct)
    {
        if (_accessor.Current is not null)
            return await next();

        var context = new RequestTelemetryContext
        {
            Command = typeof(TReq).Name,
            TraceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N"),
            SpanId = Activity.Current?.SpanId.ToString(),
            OperationId = Guid.NewGuid(),
            UserId = request is IUserScopedRequest userScoped
                ? userScoped.UserId.ToString()
                : null,
            TelegramId = request is ITelegramScopedRequest telegramScoped
                ? telegramScoped.TelegramId.ToString()
                : null
        };

        _accessor.Current = context;

        try
        {
            return await next();
        }
        finally
        {
            _accessor.Current = null;
        }
    }
}