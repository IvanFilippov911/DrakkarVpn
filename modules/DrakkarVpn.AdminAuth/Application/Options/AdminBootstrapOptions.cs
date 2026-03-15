namespace DrakkarVpn.AdminAuth.Application.Options;

public sealed class AdminBootstrapOptions
{
    public const string SectionName = "AdminBootstrap";

    public string? Email { get; set; }
    public string? Password { get; set; }
}
