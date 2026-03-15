namespace DrakkarVpn.AdminAuth.Application.Exceptions;

public sealed class AdminInactiveException : Exception
{
    public AdminInactiveException(string? message = null)
        : base(message ?? "Admin account is inactive")
    {
    }
}
