namespace DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;

public readonly record struct ServerId(Guid Value)
{
    public static ServerId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}