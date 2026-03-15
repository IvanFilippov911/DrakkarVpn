namespace DrakkarVpn.AdminAuth.Application.Exceptions;

public sealed class InvalidAdminCredentialsException : Exception
{
    public InvalidAdminCredentialsException(string? message = null)
        : base(message ?? "Invalid admin credentials")
    {
    }
}
