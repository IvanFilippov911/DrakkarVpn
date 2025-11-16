namespace DrakkarVpn.Shared.Errors.DomainErrors;

public sealed class DomainException : Exception
{
    public DomainArea Area { get; }
    public string Code { get; }

    public DomainException(DomainArea area, string code, string message)
        : base(message)
    {
        Area = area;
        Code = code;
    }

    public DomainException(Error error)
        : base(error.Message)
    {
        Area = error.Area;
        Code = error.Code;
    }
}