using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;

public sealed class XrayRuntimeService : IXrayRuntimeService
{
    public Task RestartAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
