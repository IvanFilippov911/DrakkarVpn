using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Commands;
using DrakkarVpn.Observability.Application.Features.Services.Alerts;
using DrakkarVpn.Observability.Application.Services.Alerts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.ResolveCoreAlert;

public sealed class ResolveCoreAlertHandler
    : IRequestHandler<ResolveCoreAlertCommand, bool>
{
    private readonly ICoreAlertService _service;

    public ResolveCoreAlertHandler(ICoreAlertService service)
    {
        _service = service;
    }

    public Task<bool> Handle(ResolveCoreAlertCommand c, CancellationToken ct)
        => _service.ResolveAsync(new ResolveCoreAlertArgs(
            c.Id,
            c.ResolutionType,
            c.ResolutionNote,
            c.ResolvedByAdminId), ct);
}