namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;

public sealed class ServerPollState
{
    private ServerPollState() { }

    public ServerPollState(Guid serverId)
    {
        ServerId = serverId;

        ConsecutiveFailures   = 0;
        LastKnownPeersActive  = 0;

        LastRxTotal = 0;
        LastTxTotal = 0;
        LastRxDelta = 0;
        LastTxDelta = 0;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Guid ServerId { get; private set; }
    
    public string?  LeaseOwner     { get; private set; } 
    public DateTime? LeaseUntilUtc { get; private set; }
    public DateTime? LastLeaseRenewedUtc { get; private set; }
    
    public int ConsecutiveFailures { get; private set; }
    public DateTime? BackoffUntilUtc { get; private set; }
    public DateTime? LastReachableUtc { get; private set; }
    
    public DateTime? LastPollStartedUtc  { get; private set; }
    public DateTime? LastPollFinishedUtc { get; private set; }
    public int? LastPollLatencyMs        { get; private set; }
    public bool? LastPollSuccess         { get; private set; }
    public string? LastPollErrorCode     { get; private set; } 
    
    public int  LastKnownPeersActive { get; private set; }

    public long LastRxTotal { get; private set; }
    public long LastTxTotal { get; private set; }
    public DateTime? LastTotalsAtUtc { get; private set; }

    public long LastRxDelta { get; private set; }
    public long LastTxDelta { get; private set; }
    public DateTime? LastDeltaAtUtc { get; private set; }
    
    public string? LastUpdaterInstance { get; private set; }
    
    public byte[] RowVersion { get; private set; } = null!;
    public DateTime UpdatedAtUtc { get; private set; }
    
    public void Touch(string instanceId)
    {
        LastUpdaterInstance = instanceId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}