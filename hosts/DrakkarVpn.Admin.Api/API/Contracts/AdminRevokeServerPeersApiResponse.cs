namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed class AdminRevokeServerPeersApiResponse
{
    public int Revoked { get; init; }

    public AdminRevokeServerPeersApiResponse(int revoked)
    {
        Revoked = revoked;
    }
}