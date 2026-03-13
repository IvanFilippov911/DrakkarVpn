using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Observability.Application.Features.Services.ErrorEvents;

public sealed class CoreErrorEventsQueryService : ICoreErrorEventsQueryService
{
    private readonly ICoreErrorEventRepository _repo;

    public CoreErrorEventsQueryService(ICoreErrorEventRepository repo) => _repo = repo;

    public async Task<PagedResponseDto<CoreErrorEventListItemDto>> GetPagedAsync(GetCoreErrorEventsDto q, CancellationToken ct)
    {
        var page     = q.Page <= 0 ? 1 : q.Page;
        var pageSize = Math.Clamp(q.PageSize <= 0 ? 50 : q.PageSize, 1, 200);

        var total = await _repo.CountAsync(
            q.Area, q.ErrorType, q.Command, q.DomainCode, q.UserId, q.TelegramId, q.Search, q.FromUtc, q.ToUtc, ct);

        if (total == 0)
            return PagedResponseDto<CoreErrorEventListItemDto>.Empty(page, pageSize);

        var items = await _repo.GetPagedAsync(
            page, pageSize, q.Area, q.ErrorType, q.Command, q.DomainCode, q.UserId, q.TelegramId, q.Search, q.FromUtc, q.ToUtc, ct);

        if (items.Count == 0)
            return PagedResponseDto<CoreErrorEventListItemDto>.Empty(page, pageSize);

        var dtos = items.Select(x => new CoreErrorEventListItemDto(
            Id:           x.Id,
            TimestampUtc: x.TimestampUtc,
            Command:      x.Command,
            Area:         x.Area,
            ErrorType:    x.ErrorType,
            DomainCode:   x.DomainCode,
            Message:      x.Message,
            TraceId:      x.TraceId,
            UserId:       x.UserId,
            TelegramId:   x.TelegramId
        )).ToList();

        return PagedResponseDto<CoreErrorEventListItemDto>.From(dtos, page, pageSize, total);
    }
}