using System.Diagnostics;

namespace DrakkarVpn.Observability.Application.Telemetry;

public sealed class RequestTelemetryContext
{
    public required string Command { get; init; }
    public required string TraceId { get; init; }
    public string? SpanId { get; init; }
    public required Guid OperationId { get; init; }

    public string? UserId { get; init; }
    public string? TelegramId { get; init; }

    public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
}