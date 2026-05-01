namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IXrayRuntimeService
{
    Task RestartAsync(CancellationToken ct);
}