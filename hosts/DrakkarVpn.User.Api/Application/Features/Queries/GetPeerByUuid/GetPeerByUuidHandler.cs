using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;

public sealed class GetPeerByUuidHandler
    : IRequestHandler<GetPeerByUuidQuery, GetPeerByUuidResponse?>
{
    private readonly IPeerConfigsService _svc;

    public GetPeerByUuidHandler(IPeerConfigsService svc) => _svc = svc;

    public async Task<GetPeerByUuidResponse?> Handle(GetPeerByUuidQuery q, CancellationToken ct)
    {
        var dto = await _svc.GetByAgentUuidAsync(q.PeerUuid, ct);
        if (dto is null) return null;

        return new GetPeerByUuidResponse(dto.ConfigRaw);
    }
}