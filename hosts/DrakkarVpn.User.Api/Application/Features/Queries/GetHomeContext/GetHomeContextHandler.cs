using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetHomeContext;

public sealed class GetHomeContextHandler
    : IRequestHandler<GetHomeContextRequest, HomeContextDto>
{
    private readonly IHomeContextService _svc;

    public GetHomeContextHandler(IHomeContextService svc) => _svc = svc;

    public Task<HomeContextDto> Handle(
        GetHomeContextRequest req,
        CancellationToken ct)
        => _svc.GetAsync(req.TelegramId, req.DeviceId, ct);
}

