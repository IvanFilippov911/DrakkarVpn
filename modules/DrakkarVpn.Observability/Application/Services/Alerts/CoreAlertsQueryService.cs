using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Observability.Application.DTOs.Alerts;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Observability.Application.Services.Alerts;

public sealed class CoreAlertsQueryService : ICoreAlertsQueryService
{
    private readonly ICoreAlertRepository _repo;

    public CoreAlertsQueryService(ICoreAlertRepository repo) => _repo = repo;

    public async Task<PagedResponseDto<CoreAlertListItemDto>> GetPagedAsync(CoreAlertsQueryDto q, CancellationToken ct)
    {
        var page     = q.Page <= 0 ? 1 : q.Page;
        var pageSize = Math.Clamp(q.PageSize <= 0 ? 50 : q.PageSize, 1, 200);

        var total = await _repo.CountAsync(q.IsResolved, q.Source, q.Severity, q.FromUtc, q.ToUtc, ct);
        if (total == 0)
            return PagedResponseDto<CoreAlertListItemDto>.Empty(page, pageSize);

        var items = await _repo.GetPagedAsync(page, pageSize, q.IsResolved, q.Source, q.Severity, q.FromUtc, q.ToUtc, ct);
        if (items.Count == 0)
            return PagedResponseDto<CoreAlertListItemDto>.Empty(page, pageSize);

        var dtos = items.Select(x => new CoreAlertListItemDto(
            Id:            x.Id,
            CreatedAtUtc:  x.CreatedAtUtc,
            ResolvedAtUtc: x.ResolvedAtUtc,
            IsResolved:    x.IsResolved,
            Source:        x.Source,
            Code:          x.Code,
            Severity:      x.Severity,
            Title:         x.Title,
            Message:       x.Message,
            ServerId:      x.ServerId,
            UserId:        x.UserId,
            Region:        x.Region
        )).ToList();

        return PagedResponseDto<CoreAlertListItemDto>.From(dtos, page, pageSize, total);
    }

    public Task<CoreAlertsGlobalSummaryDto> GetGlobalOpenSummaryAsync(CancellationToken ct)
        => _repo.GetGlobalOpenSummaryAsync(ct);
    
    public Task<IReadOnlyList<UserAlertDto>> GetUserLastAlertsAsync(Guid userId, int take, CancellationToken ct)
        => _repo.GetLastAlertsAsync(userId, take, ct);
    
    
}