namespace DrakkarVpn.AdminAuth.Application.Authorization;

public static class AdminRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Operator = "Operator";
    public const string ReadOnly = "ReadOnly";

    public static readonly IReadOnlyCollection<string> All = new[]
    {
        SuperAdmin,
        Operator,
        ReadOnly
    };
}
