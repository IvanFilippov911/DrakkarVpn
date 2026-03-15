namespace DrakkarVpn.AdminAuth.Application.Exceptions;

public sealed class MissingRefreshTokenException : Exception
{
    public MissingRefreshTokenException(string? message = null)
        : base(message ?? "Refresh token cookie is missing")
    {
    }
}
