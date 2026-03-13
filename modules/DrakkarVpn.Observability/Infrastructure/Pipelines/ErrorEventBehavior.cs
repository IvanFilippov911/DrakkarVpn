using System.Text.Json;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Shared.Errors.DomainErrors;
using MediatR;
using Serilog;

namespace DrakkarVpn.Observability.Infrastructure.Pipelines;

public sealed class ErrorEventBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly IRequestTelemetryContextAccessor _accessor;
    private readonly ICoreErrorEventService _errorEvents;
    private readonly ILogger _logger;

    public ErrorEventBehavior(
        IRequestTelemetryContextAccessor accessor,
        ICoreErrorEventService errorEvents)
    {
        _accessor = accessor;
        _errorEvents = errorEvents;
        _logger = Log.Logger;
    }

    public async Task<TRes> Handle(
        TReq request,
        RequestHandlerDelegate<TRes> next,
        CancellationToken ct)
    {
        var ctx = _accessor.Current
                  ?? throw new InvalidOperationException(
                      $"Telemetry context is not initialized for {typeof(TReq).Name}");

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var area = ex is DomainException dex
                ? dex.Area.ToString()
                : "System";

            try
            {
                await _errorEvents.LogAsync(new LogCoreErrorEventArgs(
                    Command: ctx.Command,
                    Area: area,
                    ErrorType: ex.GetType().Name,
                    DomainCode: (ex as DomainException)?.Code,
                    Message: ex.Message,
                    TraceId: ctx.TraceId,
                    UserId: ctx.UserId,
                    TelegramId: ctx.TelegramId,
                    PayloadJson: JsonSerializer.Serialize(request)
                ), ct);
            }
            catch (Exception telemetryEx)
            {
                _logger.Warning(
                    telemetryEx,
                    "Failed to write error event for {Command}. TraceId={TraceId}",
                    ctx.Command,
                    ctx.TraceId);
            }

            throw;
        }
    }
}