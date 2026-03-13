using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed record ResolveCoreAlertRequest(
    CoreAlertResolutionType ResolutionType,
    string? ResolutionNote
);