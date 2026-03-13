using DrakkarVpn.Observability.Application.Abstracts.Services;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Errors.DeleteCoreErrorEvent;

public sealed class DeleteCoreErrorEventHandler
    : IRequestHandler<DeleteCoreErrorEventCommand, bool>
{
    private readonly ICoreErrorEventService _service;

    public DeleteCoreErrorEventHandler(ICoreErrorEventService service)
        => _service = service;

    public Task<bool> Handle(DeleteCoreErrorEventCommand cmd, CancellationToken ct)
        => _service.DeleteAsync(cmd.Id, ct);
}