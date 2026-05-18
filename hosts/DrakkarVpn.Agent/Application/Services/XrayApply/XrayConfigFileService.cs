using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;

public sealed class XrayConfigFileService : IXrayConfigFileService
{
    public Task<XrayConfigBackupInfo> ReplaceConfigAsync(string configJson, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(new XrayConfigBackupInfo(
            ActivePath: string.Empty,
            BackupPath: null,
            HadPreviousConfig: false));
    }

    public Task RestoreBackupAsync(XrayConfigBackupInfo backup, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
