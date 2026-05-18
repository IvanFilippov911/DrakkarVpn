using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerById;

public sealed class GetServerByIdHandler
    : IRequestHandler<GetServerByIdRequest, GetServersDetailDto?>
{
    private readonly IServersQueryService _servers;

    public GetServerByIdHandler(IServersQueryService servers)
        => _servers = servers;

    public Task<GetServersDetailDto?> Handle(
        GetServerByIdRequest q,
        CancellationToken ct)
        => _servers.GetDetailAsync(q.ServerId, ct);
}