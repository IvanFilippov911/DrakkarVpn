using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IXrayConfigFileService
{
    Task<XrayConfigBackupInfo> ReplaceConfigAsync(
        string configJson,
        CancellationToken ct);

    Task RestoreBackupAsync(
        XrayConfigBackupInfo backup,
        CancellationToken ct);
}