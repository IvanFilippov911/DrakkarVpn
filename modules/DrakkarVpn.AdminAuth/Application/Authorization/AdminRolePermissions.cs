namespace DrakkarVpn.AdminAuth.Application.Authorization;

public static class AdminRolePermissions
{
    private static readonly IReadOnlySet<string> Empty = CreateSet();

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> Map =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            [AdminRoles.SuperAdmin] = CreateSet(
                AdminPermissions.UsersRead,
                AdminPermissions.UsersManage,
                AdminPermissions.ServersRead,
                AdminPermissions.ServersManage,
                AdminPermissions.PeersManage,
                AdminPermissions.SubscriptionsManage,
                AdminPermissions.TariffsManage,
                AdminPermissions.ObservabilityRead),

            [AdminRoles.Operator] = CreateSet(
                AdminPermissions.UsersRead,
                AdminPermissions.UsersManage,
                AdminPermissions.ServersRead,
                AdminPermissions.ServersManage,
                AdminPermissions.PeersManage,
                AdminPermissions.SubscriptionsManage,
                AdminPermissions.ObservabilityRead),

            [AdminRoles.ReadOnly] = CreateSet(
                AdminPermissions.UsersRead,
                AdminPermissions.ServersRead,
                AdminPermissions.ObservabilityRead)
        };

    public static IReadOnlyDictionary<string, IReadOnlySet<string>> All => Map;

    public static IReadOnlySet<string> GetPermissions(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        return Map.TryGetValue(role, out var permissions)
            ? permissions
            : Empty;
    }

    private static IReadOnlySet<string> CreateSet(params string[] permissions)
    {
        return new HashSet<string>(permissions, StringComparer.Ordinal);
    }
}
