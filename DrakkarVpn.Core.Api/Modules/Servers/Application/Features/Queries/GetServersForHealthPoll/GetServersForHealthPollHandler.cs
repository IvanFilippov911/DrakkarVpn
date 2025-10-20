using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersForHealthPoll;

public sealed class GetServersForHealthPollHandler
    : IRequestHandler<GetServersForHealthPollRequest, IReadOnlyList<GetServersForHealthPollDto>>
{
    private readonly IServerRepository _repo;

    public GetServersForHealthPollHandler(IServerRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<GetServersForHealthPollDto>> Handle(
        GetServersForHealthPollRequest request, 
        CancellationToken ct)
    {
        var servers = await _repo.GetForHealthPollAsync(ct);
        return servers
            .Select(s => new GetServersForHealthPollDto(s.Id, s.AgentBaseUrl))
            .ToList();
    }
}