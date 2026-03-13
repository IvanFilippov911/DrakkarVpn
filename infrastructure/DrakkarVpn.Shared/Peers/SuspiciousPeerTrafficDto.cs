namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record SuspiciousPeerTrafficDto(
    Guid PeerId,
    Guid ServerId,
    DateTime FromUtc,
    DateTime ToUtc,
    long TotalRxBytes,
    long TotalTxBytes,
    double? MaxSpeedMbps,
    bool   WasOnline
)
{
    public long TotalBytes => TotalRxBytes + TotalTxBytes;
}