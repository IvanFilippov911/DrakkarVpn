namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

public readonly record struct Region(string Code)
{
    public override string ToString() => Code;
}