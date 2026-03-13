using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using DrakkarVpn.Observability.Application.DTOs.Alerts;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;

public interface ICoreAlertRepository
{
    Task AddAsync(CoreAlert alert, CancellationToken ct);

    Task<IReadOnlyList<CoreAlert>> GetPagedAsync(
        int page,
        int pageSize,
        bool? isResolved,
        string? source,
        string? severity,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    Task<int> CountAsync(
        bool? isResolved,
        string? source,
        string? severity,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct);

    Task<bool> MarkResolvedAsync(
        Guid id,
        CoreAlertResolutionType resolutionType,
        string? resolutionNote,
        Guid? resolvedByAdminId,
        CancellationToken ct);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task InsertManyAsync(
        IEnumerable<CoreAlert> entities,
        CancellationToken ct);

    Task<CoreAlertsGlobalSummaryDto> GetGlobalOpenSummaryAsync(
        CancellationToken ct);

    Task<IReadOnlyList<UserAlertDto>> GetLastAlertsAsync(
        Guid userId,
        int take,
        CancellationToken ct);
}