namespace DrakkarVpn.Observability.Application.DTOs;

public sealed record CoreAlertsQueryDto(
    int Page,
    int PageSize,
    bool? IsResolved,
    string? Source,
    string? Severity,
    DateTime? FromUtc,
    DateTime? ToUtc
);