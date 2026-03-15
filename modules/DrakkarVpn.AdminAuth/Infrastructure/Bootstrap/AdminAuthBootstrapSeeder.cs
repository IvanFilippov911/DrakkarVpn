using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.AdminAuth.Application.Options;
using DrakkarVpn.AdminAuth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.AdminAuth.Infrastructure.Bootstrap;

public sealed class AdminAuthBootstrapSeeder
{
    private readonly RoleManager<AdminIdentityRole> _roleManager;
    private readonly UserManager<AdminIdentityUser> _userManager;
    private readonly AdminBootstrapOptions _options;

    public AdminAuthBootstrapSeeder(
        RoleManager<AdminIdentityRole> roleManager,
        UserManager<AdminIdentityUser> userManager,
        IOptions<AdminBootstrapOptions> options)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        await EnsureRolesAsync();
        await EnsureBootstrapAdminAsync();
    }

    private async Task EnsureRolesAsync()
    {
        foreach (var roleName in AdminRoles.All)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await _roleManager.CreateAsync(new AdminIdentityRole
            {
                Name = roleName
            });

            EnsureSucceeded(result, $"Failed to create admin role '{roleName}'");
        }
    }

    private async Task EnsureBootstrapAdminAsync()
    {
        var email = Normalize(_options.Email);
        var password = _options.Password;

        if (string.IsNullOrWhiteSpace(email))
            return;

        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "ADMIN_BOOTSTRAP_PASSWORD must be provided when ADMIN_BOOTSTRAP_EMAIL is set");

        var existingAdmin = await _userManager.FindByEmailAsync(email);
        if (existingAdmin is not null)
            return;

        var admin = new AdminIdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            LockoutEnabled = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(admin, password);
        EnsureSucceeded(createResult, "Failed to create bootstrap admin user");

        var addToRoleResult = await _userManager.AddToRoleAsync(admin, AdminRoles.SuperAdmin);
        EnsureSucceeded(addToRoleResult, "Failed to assign SuperAdmin role to bootstrap admin user");
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
        throw new InvalidOperationException($"{message}. {errors}");
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
