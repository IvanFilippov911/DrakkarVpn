namespace DrakkarVpn.Shared.Errors;

public sealed class PeersRevokeFailedException : Exception
{
    public string Scope { get; }    
    public Guid   ScopeId { get; }
    public int    Revoked { get; }
    public int    Failed  { get; }

    private PeersRevokeFailedException(
        string scope,
        Guid scopeId,
        int revoked,
        int failed)
        : base($"Failed to revoke {failed} peers for {scope} {scopeId}. Revoked: {revoked}")
    {
        Scope   = scope;
        ScopeId = scopeId;
        Revoked = revoked;
        Failed  = failed;
    }

    public static PeersRevokeFailedException ForUser(Guid userId, int revoked, int failed) =>
        new("user", userId, revoked, failed);

    public static PeersRevokeFailedException ForServer(Guid serverId, int revoked, int failed) =>
        new("server", serverId, revoked, failed);
}