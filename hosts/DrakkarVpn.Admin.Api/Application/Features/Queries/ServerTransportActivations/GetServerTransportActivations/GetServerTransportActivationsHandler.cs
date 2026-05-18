using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportActivations;

public sealed class GetServerTransportActivationsHandler
    : IRequestHandler<GetServerTransportActivationsQuery, IReadOnlyList<ServerTransportActivationListItemDto>>
{
    private readonly IServerTransportActivationQueryService _service;

    public GetServerTransportActivationsHandler(IServerTransportActivationQueryService service)
        => _service = service;

    public Task<IReadOnlyList<ServerTransportActivationListItemDto>> Handle(
        GetServerTransportActivationsQuery query,
        CancellationToken ct)
        => _service.GetByServerAsync(query.ServerId, ct);
}
