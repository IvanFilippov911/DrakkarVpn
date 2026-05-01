namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record XrayConfigBackupInfo(
    string ActivePath,
    string? BackupPath,
    bool HadPreviousConfig);