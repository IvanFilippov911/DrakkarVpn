using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Infrastructure.Auth;

public sealed class AdminAuthService : IAdminAuthService
{
    private readonly IAdminAuthenticationService _authenticationService;
    private readonly IAdminJwtTokenService _jwtTokenService;
    private readonly IAdminRefreshSessionService _refreshSessionService;
    private readonly IAdminCurrentProfileService _currentProfileService;
    private readonly IAdminProfileFactory _profileFactory;

    public AdminAuthService(
        IAdminAuthenticationService authenticationService,
        IAdminJwtTokenService jwtTokenService,
        IAdminRefreshSessionService refreshSessionService,
        IAdminCurrentProfileService currentProfileService,
        IAdminProfileFactory profileFactory)
    {
        _authenticationService = authenticationService;
        _jwtTokenService = jwtTokenService;
        _refreshSessionService = refreshSessionService;
        _currentProfileService = currentProfileService;
        _profileFactory = profileFactory;
    }

    public async Task<AdminLoginResult> LoginAsync(AdminLoginRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var admin = await _authenticationService.AuthenticateAsync(request.Email, request.Password, ct);
        var profile = _profileFactory.Create(admin.AdminId, admin.Email, admin.Roles);
        var accessToken = _jwtTokenService.Issue(admin);
        var refreshSession = await _refreshSessionService.CreateAsync(
            admin.AdminId,
            request.IpAddress,
            request.UserAgent,
            ct);

        return new AdminLoginResult(
            new AdminSessionTokens(
                AccessToken: accessToken.Value,
                AccessTokenExpiresAtUtc: accessToken.ExpiresAtUtc,
                AccessTokenId: accessToken.Jti,
                RefreshToken: refreshSession.RefreshToken,
                RefreshTokenExpiresAtUtc: refreshSession.ExpiresAtUtc),
            profile);
    }

    public async Task<AdminRefreshSessionResult> RefreshAsync(
        AdminRefreshSessionRequest request,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var replacementSession = await _refreshSessionService.RotateAsync(
            request.RefreshToken,
            request.IpAddress,
            request.UserAgent,
            ct);
        var admin = await _authenticationService.GetActiveAdminAsync(replacementSession.AdminUserId, ct);
        var profile = _profileFactory.Create(admin.AdminId, admin.Email, admin.Roles);
        var accessToken = _jwtTokenService.Issue(admin);

        return new AdminRefreshSessionResult(
            new AdminSessionTokens(
                AccessToken: accessToken.Value,
                AccessTokenExpiresAtUtc: accessToken.ExpiresAtUtc,
                AccessTokenId: accessToken.Jti,
                RefreshToken: replacementSession.RefreshToken,
                RefreshTokenExpiresAtUtc: replacementSession.ExpiresAtUtc),
            profile);
    }

    public async Task LogoutAsync(AdminLogoutSessionRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _refreshSessionService.RevokeAsync(request.RefreshToken, ct);
    }

    public async Task LogoutAllAsync(CancellationToken ct)
    {
        var currentAdmin = await _currentProfileService.GetCurrentAsync(ct);
        await _refreshSessionService.RevokeAllAsync(currentAdmin.AdminId, ct);
    }

    public Task<AdminCurrentAdminProfile> GetCurrentAdminAsync(CancellationToken ct)
    {
        return _currentProfileService.GetCurrentAsync(ct);
    }
}
