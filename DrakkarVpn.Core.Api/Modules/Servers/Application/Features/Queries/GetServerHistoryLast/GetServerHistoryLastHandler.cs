using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistoryLast;

public sealed class GetServerHistoryLastHandler
    : IRequestHandler<GetServerHistoryLastRequest, IReadOnlyList<ServerMetricsHistoryDto>>
{
    private readonly IMediator _mediator;
    public GetServerHistoryLastHandler(IMediator mediator) => _mediator = mediator;

    public Task<IReadOnlyList<ServerMetricsHistoryDto>> Handle(GetServerHistoryLastRequest q, CancellationToken ct)
    {
        var toUtc = DateTime.UtcNow;
        var fromUtc = toUtc.AddMinutes(-q.Minutes);
        return _mediator.Send(new GetServerHistoryRequest(q.ServerId, fromUtc, toUtc), ct);
    }
}