using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Services.Alerts.Factories.Exceptions;

public interface IExceptionAlertFactory
{
    IEnumerable<CreateCoreAlertArgs> Build(
        Exception ex,
        string command,
        string area,
        Guid? userId,
        string traceId,
        long elapsedMs);
}