namespace DrakkarVpn.Shared.Errors.DomainErrors;

public sealed record Error(DomainArea Area, string Code, string Message)
{
    public DomainException ToException() => new(this);
}