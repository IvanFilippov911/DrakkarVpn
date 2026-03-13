using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreErrorEvents;

public sealed record GetCoreErrorEventsQuery(
    int Page,
    int PageSize,
    string? Area,
    string? ErrorType,
    string? Command,
    string? DomainCode,
    string? UserId,
    string? TelegramId,
    string? Search,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IRequest<PagedResponseDto<CoreErrorEventListItemDto>>;