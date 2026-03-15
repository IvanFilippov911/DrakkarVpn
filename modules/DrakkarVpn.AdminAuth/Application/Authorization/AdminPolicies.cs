namespace DrakkarVpn.AdminAuth.Application.Authorization;

public static class AdminPolicies
{
    public const string UsersRead = AdminPermissions.UsersRead;
    public const string UsersManage = AdminPermissions.UsersManage;
    public const string ServersRead = AdminPermissions.ServersRead;
    public const string ServersManage = AdminPermissions.ServersManage;
    public const string PeersManage = AdminPermissions.PeersManage;
    public const string SubscriptionsManage = AdminPermissions.SubscriptionsManage;
    public const string TariffsManage = AdminPermissions.TariffsManage;
    public const string ObservabilityRead = AdminPermissions.ObservabilityRead;
}
