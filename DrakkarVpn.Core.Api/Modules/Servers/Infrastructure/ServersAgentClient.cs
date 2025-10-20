using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;

public sealed class ServersAgentClient : BaseAgentClient, IServersAgentClient
{
    private readonly ILogger<ServersAgentClient> _logger;
    private readonly IServerRepository _repo;

    public ServersAgentClient(HttpClient http, ILogger<ServersAgentClient> logger, IServerRepository repo)
        : base(http)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<BenchmarkResultDto> RunBenchmarkAsync(Guid serverId, CancellationToken ct)
    {
        var server = await _repo.GetAsync(new ServerId(serverId), ct);
        var url = $"{server.AgentBaseUrl}benchmark";
        
        try
        {
            var result = await GetAsync<BenchmarkResultDto>(url, ct);
            return result;
        }
        catch (Exception ex)
        {
            return new BenchmarkResultDto(-1, -1, DateTimeOffset.UtcNow);
        }
    }
}