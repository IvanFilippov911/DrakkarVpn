
using DrakkarVpn.Observability.Application.Telemetry;

namespace DrakkarVpn.Observability.Application.Abstracts.Telemetry;

public interface IRequestTelemetryContextAccessor
{
    RequestTelemetryContext? Current { get; set; }
}