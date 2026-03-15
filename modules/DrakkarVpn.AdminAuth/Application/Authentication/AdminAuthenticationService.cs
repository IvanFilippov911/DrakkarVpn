using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using DrakkarVpn.AdminAuth.Application.Exceptions;
using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DrakkarVpn.AdminAuth.Application.Authentication;

public sealed class AdminAuthenticationService : IAdminAuthenticationService
{
    private readonly UserManager<AdminIdentityUser> _userManager;

    public AdminAuthenticationService(UserManager<AdminIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AdminJwtTokenDescriptor> AuthenticateAsync(
        string email,
        string password,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ct.ThrowIfCancellationRequested();

        var admin = await _userManager.FindByEmailAsync(email.Trim());
        if (admin is null)
            throw new InvalidAdminCredentialsException();

        if (!admin.LockoutEnabled)
        {
            var enableLockoutResult = await _userManager.SetLockoutEnabledAsync(admin, true);
            EnsureSucceeded(enableLockoutResult, "Failed to enable admin lockout");
        }

        if (await _userManager.IsLockedOutAsync(admin))
            throw new InvalidAdminCredentialsException("Admin authentication failed");

        var passwordValid = await _userManager.CheckPasswordAsync(admin, password);
        if (!passwordValid)
        {
            var failedAccessResult = await _userManager.AccessFailedAsync(admin);
            EnsureSucceeded(failedAccessResult, "Failed to register admin access failure");
            throw new InvalidAdminCredentialsException();
        }

        EnsureActive(admin);

        var resetAccessFailedResult = await _userManager.ResetAccessFailedCountAsync(admin);
        EnsureSucceeded(resetAccessFailedResult, "Failed to reset admin access failure count");

        admin.LastLoginAtUtc = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(admin);
        EnsureSucceeded(updateResult, "Admin login state update failed");

        var roles = (await _userManager.GetRolesAsync(admin)).ToArray();
        return new AdminJwtTokenDescriptor(admin.Id, admin.Email!, roles);
    }

    public async Task<AdminJwtTokenDescriptor> GetActiveAdminAsync(
        Guid adminId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var admin = await _userManager.FindByIdAsync(adminId.ToString());
        if (admin is null)
            throw new InvalidAdminCredentialsException("Admin authentication failed");

        EnsureActive(admin);

        var roles = (await _userManager.GetRolesAsync(admin)).ToArray();
        return new AdminJwtTokenDescriptor(admin.Id, admin.Email!, roles);
    }

    private static void EnsureActive(AdminIdentityUser admin)
    {
        if (!admin.IsActive)
            throw new AdminInactiveException();

        if (string.IsNullOrWhiteSpace(admin.Email))
            throw new InvalidAdminCredentialsException("Admin authentication failed");
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
        throw new InvalidOperationException($"{message}. {errors}");
    }
}
