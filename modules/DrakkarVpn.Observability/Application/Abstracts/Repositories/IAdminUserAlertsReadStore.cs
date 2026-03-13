
using DrakkarVpn.Observability.Application.DTOs.Alerts;

namespace DrakkarVpn.Observability.Application.Abstracts.Repositories;

public interface IAdminUserAlertsReadStore
{
    Task<IReadOnlyList<UserAlertDto>> GetLastAlertsAsync(
        Guid userId,
        int  take,
        CancellationToken ct);
}