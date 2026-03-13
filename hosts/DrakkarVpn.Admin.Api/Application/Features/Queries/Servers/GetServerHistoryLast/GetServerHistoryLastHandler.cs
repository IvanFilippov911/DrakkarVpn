using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerHistoryLast;

public sealed class GetServerHistoryLastHandler
    : IRequestHandler<GetServerHistoryLastRequest, IReadOnlyList<ServerMetricsHistoryDto>>
{
    private readonly IServersQueryService _servers;

    public GetServerHistoryLastHandler(IServersQueryService servers)
        => _servers = servers;

    public Task<IReadOnlyList<ServerMetricsHistoryDto>> Handle(
        GetServerHistoryLastRequest q,
        CancellationToken ct)
        => _servers.GetHistoryLastAsync(q.ServerId, q.Minutes, ct);
}