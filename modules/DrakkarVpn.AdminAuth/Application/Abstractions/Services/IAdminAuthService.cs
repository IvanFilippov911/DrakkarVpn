using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Application.Abstractions.Services;

public interface IAdminAuthService
{
    Task<AdminLoginResult> LoginAsync(AdminLoginRequest request, CancellationToken ct);

    Task<AdminRefreshSessionResult> RefreshAsync(
        AdminRefreshSessionRequest request,
        CancellationToken ct);

    Task LogoutAsync(AdminLogoutSessionRequest request, CancellationToken ct);

    Task LogoutAllAsync(CancellationToken ct);

    Task<AdminCurrentAdminProfile> GetCurrentAdminAsync(CancellationToken ct);
}
