using Serilog;
using Serilog.Context;
using System.Diagnostics;
using MediatR;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersForHealthPoll;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetExpiredSubscriptions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpsertServerMetricsHistory;

namespace DrakkarVpn.Core.Pipeline;

public sealed class LoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    private readonly Serilog.ILogger _logger;

    public LoggingBehavior()
    {
        _logger = Log.Logger;
    }

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (typeof(TReq).Name is nameof(GetServersForHealthPollRequest) 
            or nameof(GetExpiredSubscriptionsRequest) 
            or nameof(EvaluateServerHealthRequest) 
            or nameof(ExpireSubscriptionRequest)
            or nameof(GetServerByIdRequest)
            or nameof(UpsertServerMetricsHistoryRequest))
        {
            return await next();
        }

        var command = typeof(TReq).Name;

        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        var spanId = Activity.Current?.SpanId.ToString();
        var operationId = Guid.NewGuid();

        var userId = request?.GetType().GetProperty("UserId")?.GetValue(request)?.ToString();
        var tgId = request?.GetType().GetProperty("TelegramId")?.GetValue(request)?.ToString();

        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("SpanId", spanId))
        using (LogContext.PushProperty("Command", command))
        using (LogContext.PushProperty("OperationId", operationId))
        using (LogContext.PushProperty("Service", "Drakkar.Core"))
        using (LogContext.PushProperty("Component", "Core.API"))
        using (LogContext.PushProperty("Env", "prod"))
        using (LogContext.PushProperty("Region", "eu"))
        using (LogContext.PushProperty("UserId", string.IsNullOrWhiteSpace(userId) ? null : userId))
        using (LogContext.PushProperty("TelegramId", string.IsNullOrWhiteSpace(tgId) ? null : tgId))
        {
            var sw = Stopwatch.StartNew();
            _logger.Information("▶️ {Command} started {@Request}", command, request);

            try
            {
                var response = await next();
                sw.Stop();

                _logger.Information("✅ {Command} completed in {ElapsedMs}ms",
                    command, sw.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.ForContext("Error", true)
                    .Error(ex, "❌ {Command} failed after {ElapsedMs}ms: {ErrorMessage}",
                        command, sw.ElapsedMilliseconds, ex.Message);
                throw;
            }
        }
    }
}