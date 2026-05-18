using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;

public sealed class XrayHealthCheckService : IXrayHealthCheckService
{
    public Task EnsureHealthyAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
