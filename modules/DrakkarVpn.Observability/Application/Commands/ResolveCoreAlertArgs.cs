using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Observability.Application.Commands;

public sealed record ResolveCoreAlertArgs(
    Guid Id,
    CoreAlertResolutionType ResolutionType,
    string? ResolutionNote,
    Guid? ResolvedByAdminId
);