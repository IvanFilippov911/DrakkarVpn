namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IAgentApplyServerTransportService
{
    Task ApplyAsync(Guid serverId, Guid activationId, CancellationToken ct);
}