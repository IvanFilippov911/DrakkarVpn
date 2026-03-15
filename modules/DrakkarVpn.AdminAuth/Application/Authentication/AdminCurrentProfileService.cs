using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;

namespace DrakkarVpn.AdminAuth.Infrastructure.Auth;

public sealed class AdminCurrentProfileService : IAdminCurrentProfileService
{
    private readonly ICurrentAdminAccessor _currentAdminAccessor;

    public AdminCurrentProfileService(ICurrentAdminAccessor currentAdminAccessor)
    {
        _currentAdminAccessor = currentAdminAccessor;
    }

    public Task<AdminCurrentAdminProfile> GetCurrentAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(_currentAdminAccessor.GetRequiredCurrent());
    }
}
