using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.ResolveCoreAlert;

public sealed record ResolveCoreAlertCommand(
    Guid Id,
    CoreAlertResolutionType ResolutionType,
    string? ResolutionNote,
    Guid? ResolvedByAdminId
) : IRequest<bool>;