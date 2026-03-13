using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerManagementService
{
    Task<Guid> RegisterAsync(
        string name,
        string region,
        string publicHost,
        string agentBaseUrl,
        string agentTokenEncrypted,
        int? maxPeers,
        CancellationToken ct);

    Task<DeleteServerResult> DeleteAsync(Guid serverId, CancellationToken ct);
}