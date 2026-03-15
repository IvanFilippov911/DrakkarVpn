using DrakkarVpn.AdminAuth.Application.Abstractions.Services;
using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Auth.GetCurrentAdmin;

public sealed class GetCurrentAdminHandler : IRequestHandler<GetCurrentAdminQuery, AdminCurrentAdminProfile>
{
    private readonly IAdminAuthService _adminAuthService;

    public GetCurrentAdminHandler(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    public Task<AdminCurrentAdminProfile> Handle(GetCurrentAdminQuery request, CancellationToken ct)
    {
        return _adminAuthService.GetCurrentAdminAsync(ct);
    }
}
