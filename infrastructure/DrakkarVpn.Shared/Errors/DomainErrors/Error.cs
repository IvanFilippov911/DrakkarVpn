namespace DrakkarVpn.Shared.Errors.DomainErrors;

public sealed record Error(
    DomainArea Area,
    string Code,
    string Message,
    DomainErrorType ErrorType = DomainErrorType.Warning
)
{
    public DomainException ToException() => new(this);
}