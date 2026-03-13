using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Features.Services.Alerts;
using DrakkarVpn.Observability.Application.Services.Alerts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.DeleteCoreAlert;

public sealed class DeleteCoreAlertHandler
    : IRequestHandler<DeleteCoreAlertCommand, bool>
{
    private readonly ICoreAlertService _service;

    public DeleteCoreAlertHandler(ICoreAlertService service)
    {
        _service = service;
    }

    public Task<bool> Handle(DeleteCoreAlertCommand c, CancellationToken ct)
        => _service.DeleteAsync(c.Id, ct);
}