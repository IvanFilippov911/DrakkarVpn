namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public enum DeleteTransportProfileResult
{
    NotFound = 1,
    InUse = 2,
    Deleted = 3
}
