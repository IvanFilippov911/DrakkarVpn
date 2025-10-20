using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.BenchmarkServer;

public sealed class BenchmarkServerHandler : IRequestHandler<BenchmarkServerRequest, BenchmarkResultDto>
{
    private readonly IServersAgentClient _agentClient;
    private readonly IServerRepository _repo;
    private readonly ILogger<BenchmarkServerHandler> _logger;

    public BenchmarkServerHandler(
        IServersAgentClient agentClient,
        IServerRepository repo,
        ILogger<BenchmarkServerHandler> logger)
    {
        _agentClient = agentClient;
        _repo = repo;
        _logger = logger;
    }

    public async Task<BenchmarkResultDto> Handle(BenchmarkServerRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Running benchmark on server {id}", req.ServerId);

        var result = await _agentClient.RunBenchmarkAsync(req.ServerId, ct);

        if (result.DownloadMbps > 0)
        {
            await _repo.UpdateBenchmarkAsync(req.ServerId, result, ct);
        }

        return result;
    }
}