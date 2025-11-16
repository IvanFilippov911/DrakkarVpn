namespace DrakkarVpn.Shared.Errors;

public sealed class PeersRevokeFailedException : Exception
{
    public Guid UserId { get; }
    public int Revoked { get; }
    public int Failed  { get; }

    public PeersRevokeFailedException(Guid userId, int revoked, int failed)
        : base($"Failed to revoke {failed} peers for user {userId}. Revoked: {revoked}")
    {
        UserId = userId;
        Revoked = revoked;
        Failed  = failed;
    }
}