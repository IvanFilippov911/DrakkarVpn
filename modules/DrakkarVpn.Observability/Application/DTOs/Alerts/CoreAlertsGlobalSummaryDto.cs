namespace DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

public sealed record CoreAlertsGlobalSummaryDto(
    int TotalOpen,
    int Critical,
    int Warning,
    int Info
);