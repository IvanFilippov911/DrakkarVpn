using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.DTOs;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreErrorEvents;

public sealed class GetCoreErrorEventsHandler
    : IRequestHandler<GetCoreErrorEventsQuery, PagedResponseDto<CoreErrorEventListItemDto>>
{
    private readonly ICoreErrorEventsQueryService _service;

    public GetCoreErrorEventsHandler(ICoreErrorEventsQueryService service)
        => _service = service;

    public Task<PagedResponseDto<CoreErrorEventListItemDto>> Handle(
        GetCoreErrorEventsQuery q,
        CancellationToken ct)
    {
        var dto = new GetCoreErrorEventsDto(
            Page:       q.Page,
            PageSize:   q.PageSize,
            Area:       q.Area,
            ErrorType:  q.ErrorType,
            Command:    q.Command,
            DomainCode: q.DomainCode,
            UserId:     q.UserId,
            TelegramId: q.TelegramId,
            Search:     q.Search,
            FromUtc:    q.FromUtc,
            ToUtc:      q.ToUtc
        );

        return _service.GetPagedAsync(dto, ct);
    }
}