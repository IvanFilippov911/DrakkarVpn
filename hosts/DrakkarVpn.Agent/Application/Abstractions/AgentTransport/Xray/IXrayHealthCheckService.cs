namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IXrayHealthCheckService
{
    Task EnsureHealthyAsync(CancellationToken ct);
}