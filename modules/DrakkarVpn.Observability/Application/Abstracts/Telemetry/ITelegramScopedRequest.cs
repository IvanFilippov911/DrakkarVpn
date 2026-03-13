namespace DrakkarVpn.Observability.Application.Abstracts.Telemetry;

public interface ITelegramScopedRequest
{
    long TelegramId { get; }
}