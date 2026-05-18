namespace DrakkarVpn.Servers.Domain.VO;

public readonly record struct PublicHost(string Value)
{
    public override string ToString() => Value;
}