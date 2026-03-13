using DrakkarVpn.Core.Api.Diagnostics;
using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using DrakkarVpn.Shared.Errors.DomainErrors;
using MediatR;

namespace DrakkarVpn.Observability.Infrastructure.Pipelines;

public sealed class RequestMetricsBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly IRequestTelemetryContextAccessor _accessor;
    private readonly ICoreRequestMetricsSink _metrics;

    public RequestMetricsBehavior(
        IRequestTelemetryContextAccessor accessor,
        ICoreRequestMetricsSink metrics)
    {
        _accessor = accessor;
        _metrics = metrics;
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
            var response = await next();

            _metrics.TrackSuccess(
                ctx.Command,
                ctx.Stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            var area = ex is DomainException dex
                ? dex.Area.ToString()
                : "System";

            _metrics.TrackError(
                ctx.Command,
                ctx.Stopwatch.ElapsedMilliseconds,
                area);

            throw;
        }
    }
}