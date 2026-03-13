using DrakkarVpn.Observability.Application.Abstracts.Telemetry;
using DrakkarVpn.Observability.Application.Telemetry;

namespace DrakkarVpn.Observability.Infrastructure.Telemetry;

public sealed class RequestTelemetryContextAccessor : IRequestTelemetryContextAccessor
{
    private static readonly AsyncLocal<RequestTelemetryContext?> CurrentHolder = new();

    public RequestTelemetryContext? Current
    {
        get => CurrentHolder.Value;
        set => CurrentHolder.Value = value;
    }
}