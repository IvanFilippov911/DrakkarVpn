using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using MediatR;
using Serilog;
using Serilog.Context;

namespace DrakkarVpn.Observability.Infrastructure.Pipelines;

public sealed class RequestLoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly ILogger _logger;
    private readonly IRequestTelemetryContextAccessor _accessor;

    public RequestLoggingBehavior(IRequestTelemetryContextAccessor accessor)
    {
        _logger = Log.Logger;
        _accessor = accessor;
    }

    public async Task<TRes> Handle(
        TReq request,
        RequestHandlerDelegate<TRes> next,
        CancellationToken ct)
    {
        var ctx = _accessor.Current
                  ?? throw new InvalidOperationException(
                      $"Telemetry context is not initialized for {typeof(TReq).Name}");

        using (LogContext.PushProperty("TraceId", ctx.TraceId))
        using (LogContext.PushProperty("SpanId", ctx.SpanId))
        using (LogContext.PushProperty("Command", ctx.Command))
        using (LogContext.PushProperty("OperationId", ctx.OperationId))
        using (LogContext.PushProperty("Service", "Drakkar.Core"))
        using (LogContext.PushProperty("Component", "Core.API"))
        using (LogContext.PushProperty("UserId", ctx.UserId))
        using (LogContext.PushProperty("TelegramId", ctx.TelegramId))
        {
            _logger.Information("▶️ {Command} started {@Request}", ctx.Command, request);

            try
            {
                var response = await next();

                _logger.Information(
                    "✅ {Command} completed in {ElapsedMs}ms",
                    ctx.Command,
                    ctx.Stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ForContext("Error", true)
                    .Error(
                        ex,
                        "❌ {Command} failed after {ElapsedMs}ms: {ErrorMessage}",
                        ctx.Command,
                        ctx.Stopwatch.ElapsedMilliseconds,
                        ex.Message);

                throw;
            }
        }
    }
}