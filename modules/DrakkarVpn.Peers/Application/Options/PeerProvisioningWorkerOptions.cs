namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Options;

public sealed class PeerProvisioningWorkerOptions
{
    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(1);

    public int BatchSize { get; init; } = 30;
    public int HttpConcurrency { get; init; } = 30;
    
    public TimeSpan LeaseDuration { get; init; } = TimeSpan.FromSeconds(15);
    public TimeSpan StuckTimeout { get; init; } = TimeSpan.FromMinutes(2);
    
    public TimeSpan ErrorBackoff { get; init; } = TimeSpan.FromSeconds(1);
    
    public int StartupJitterMs { get; init; } = 250;
}