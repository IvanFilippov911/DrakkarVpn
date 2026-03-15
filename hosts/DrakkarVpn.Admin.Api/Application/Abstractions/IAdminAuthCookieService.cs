namespace DrakkarVpn.Core.Api.Modules.Admin.API.Auth;

public interface IAdminAuthCookieService
{
    string GetRequiredRefreshToken();
    bool TryGetRefreshToken(out string refreshToken);
    void SetRefreshToken(string refreshToken, DateTime expiresAtUtc);
    void DeleteRefreshToken();
}
