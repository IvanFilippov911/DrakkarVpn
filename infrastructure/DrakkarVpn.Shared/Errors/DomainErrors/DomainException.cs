namespace DrakkarVpn.Shared.Errors.DomainErrors;

public sealed class DomainException : Exception
{
    public DomainArea Area { get; }
    public string Code { get; }
    public DomainErrorType ErrorType { get; } = DomainErrorType.Warning;

    public DomainException(DomainArea area, string code, string message,
        DomainErrorType errorType = DomainErrorType.Warning)
        : base(message)
    {
        Area = area;
        Code = code;
        ErrorType = errorType;
    }

    public DomainException(Error error)
        : base(error.Message)
    {
        Area = error.Area;
        Code = error.Code;
        ErrorType = error.ErrorType; 
    }
}