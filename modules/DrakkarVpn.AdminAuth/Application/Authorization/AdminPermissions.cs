namespace DrakkarVpn.AdminAuth.Application.Authorization;

public static class AdminPermissions
{
    public const string UsersRead = "admin.users.read";
    public const string UsersManage = "admin.users.manage";
    public const string ServersRead = "admin.servers.read";
    public const string ServersManage = "admin.servers.manage";
    public const string PeersManage = "admin.peers.manage";
    public const string SubscriptionsManage = "admin.subscriptions.manage";
    public const string TariffsManage = "admin.tariffs.manage";
    public const string ObservabilityRead = "admin.observability.read";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        UsersRead,
        UsersManage,
        ServersRead,
        ServersManage,
        PeersManage,
        SubscriptionsManage,
        TariffsManage,
        ObservabilityRead
    };
}
