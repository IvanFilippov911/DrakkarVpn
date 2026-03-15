namespace DrakkarVpn.AdminAuth.Application.Exceptions;

public sealed class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException(string? message = null)
        : base(message ?? "Invalid refresh token")
    {
    }
}
