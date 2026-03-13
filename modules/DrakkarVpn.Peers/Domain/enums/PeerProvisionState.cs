namespace DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;

public enum PeerProvisionState : short
{
    Pending = 0,
    Processing = 1,
    Ready = 2,
    Failed = 3
}