using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IServerManagementService
{
    Task<Guid> RegisterAsync(
        string name,
        string region,
        string publicHost,
        int publicPort,
        string agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers,
        CancellationToken ct);

    Task<DeleteServerResult> DeleteAsync(Guid serverId, CancellationToken ct);
}