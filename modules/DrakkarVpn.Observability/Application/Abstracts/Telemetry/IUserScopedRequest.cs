namespace DrakkarVpn.Observability.Application.Abstracts.Telemetry;

public interface IUserScopedRequest
{
    Guid UserId { get; }
}