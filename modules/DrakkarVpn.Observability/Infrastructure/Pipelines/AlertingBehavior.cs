using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using DrakkarVpn.Observability.Application.Services.Alerts.Factories.Exceptions;
using DrakkarVpn.Shared.Errors.DomainErrors;
using MediatR;
using Serilog;

namespace DrakkarVpn.Observability.Infrastructure.Pipelines;

public sealed class AlertingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly IRequestTelemetryContextAccessor _accessor;
    private readonly ICoreAlertService _alertsService;
    private readonly IExceptionAlertFactory _exceptionAlerts;
    private readonly ILogger _logger;

    public AlertingBehavior(
        IRequestTelemetryContextAccessor accessor,
        ICoreAlertService alertsService,
        IExceptionAlertFactory exceptionAlerts)
    {
        _accessor = accessor;
        _alertsService = alertsService;
        _exceptionAlerts = exceptionAlerts;
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

            Guid? userId = Guid.TryParse(ctx.UserId, out var parsedUserId)
                ? parsedUserId
                : null;

            var alertArgs = _exceptionAlerts.Build(
                    ex,
                    ctx.Command,
                    area,
                    userId,
                    ctx.TraceId,
                    ctx.Stopwatch.ElapsedMilliseconds)
                .ToList();

            if (alertArgs.Count > 0)
            {
                try
                {
                    await _alertsService.CreateBatchAsync(alertArgs, ct);
                }
                catch (Exception alertEx)
                {
                    _logger.Warning(
                        alertEx,
                        "Failed to create alerts for {Command}. TraceId={TraceId}",
                        ctx.Command,
                        ctx.TraceId);
                }
            }

            throw;
        }
    }
}