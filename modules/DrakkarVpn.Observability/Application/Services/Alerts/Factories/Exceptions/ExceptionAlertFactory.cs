using System.Text.Json;
using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.Services.Alerts.Factories.Exceptions;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Observability.Application.Features.Services.Alerts.Factories.Exceptions;

public sealed class ExceptionAlertFactory : IExceptionAlertFactory
{
    public IEnumerable<CreateCoreAlertArgs> Build(
        Exception ex,
        string command,
        string area,
        Guid? userId,
        string traceId,
        long elapsedMs)
    {
        if (ex is DomainException dex)
        {
            if (dex.ErrorType == DomainErrorType.Critical)
            {
                yield return MakeAlert(
                    source:   "Core",
                    code:     "DOMAIN_CRITICAL_ERROR",
                    severity: "Critical",
                    title:    $"Критическая доменная ошибка в {command}",
                    message:  dex.Message,
                    userId:   userId,
                    details: new
                    {
                        Command   = command,
                        Area      = area,
                        TraceId   = traceId,
                        Code      = dex.Code,
                        ElapsedMs = elapsedMs
                    }
                );
            }

            yield break;
        }

        if (string.Equals(area, "System", StringComparison.OrdinalIgnoreCase))
        {
            yield return MakeAlert(
                source:   "Core",
                code:     "SYSTEM_EXCEPTION",
                severity: "Warning",
                title:    $"Системная ошибка в {command}",
                message:  ex.Message,
                userId:   userId,
                details: new
                {
                    Command       = command,
                    Area          = area,
                    TraceId       = traceId,
                    ExceptionType = ex.GetType().Name,
                    ElapsedMs     = elapsedMs
                }
            );
        }
    }

    private static CreateCoreAlertArgs MakeAlert(
        string source,
        string code,
        string severity,
        string title,
        string message,
        Guid? userId,
        object? details)
    {
        var detailsJson = details is null ? null : JsonSerializer.Serialize(details);

        return new CreateCoreAlertArgs(
            Source:      source,
            Code:        code,
            Severity:    severity,
            Title:       title,
            Message:     message,
            ServerId:    null,
            UserId:      userId,
            Region:      null,
            DetailsJson: detailsJson
        );
    }
}