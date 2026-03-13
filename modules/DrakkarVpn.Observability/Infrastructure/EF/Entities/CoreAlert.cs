using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;

public sealed class CoreAlert
{
    public Guid Id { get; set; }

    public DateTime  CreatedAtUtc   { get; set; }
    public DateTime? ResolvedAtUtc  { get; set; }
    public bool      IsResolved     { get; set; }

    /// <summary>Источник: "Server", "CoreHealth", "UserTraffic" и т.п.</summary>
    public string Source { get; set; } = default!;

    /// <summary>Машинный код алерта, например "SERVER_STATUS_CHANGED"</summary>
    public string Code { get; set; } = default!;

    /// <summary>"Info" / "Warning" / "Critical"</summary>
    public string Severity { get; set; } = default!;

    public string Title   { get; set; } = default!;
    public string Message { get; set; } = default!;

    public Guid?   ServerId { get; set; }
    public Guid?   UserId   { get; set; }
    public string? Region   { get; set; }

    /// <summary>Доп. данные (скорости, latency, старый/новый статус) в jsonb</summary>
    public string? DetailsJson { get; set; }

    // --- Новые поля резолва ---

    public CoreAlertResolutionType? ResolutionType { get; set; }
    public string?                  ResolutionNote { get; set; }
    public Guid?                    ResolvedByAdminId { get; set; }
}