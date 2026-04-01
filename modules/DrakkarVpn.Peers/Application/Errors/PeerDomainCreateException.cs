namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Errors;

public sealed class PeerDomainCreateException : Exception
{
    public PeerDomainCreateException(
        string errorCode,
        string message,
        bool isTransient,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        IsTransient = isTransient;
    }

    public string ErrorCode { get; }
    public bool IsTransient { get; }
}
