using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.EvaluateServerHealth;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Infrastructure.BackgroundWorkers;

public sealed class ServerHealthBackgroundWorker : BackgroundService
{
    private readonly ILogger<ServerHealthBackgroundWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    
    private readonly Dictionary<Guid, int> _lastPeersCount = new();

    public ServerHealthBackgroundWorker(
        ILogger<ServerHealthBackgroundWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var servers = await mediator.Send(new GetServersRequest(null, null), stoppingToken);

                foreach (var server in servers)
                {
                    var client = _httpClientFactory.CreateClient();
                    AgentHealthDto health;

                    try
                    {
                        health = await client.GetFromJsonAsync<AgentHealthDto>(
                            $"{server.AgentBaseUrl}health",
                            stoppingToken
                        ) ?? new AgentHealthDto(false, GetLastPeers(server.Id));
                    }
                    catch(Exception ex)
                    {
                        health = new AgentHealthDto(false, GetLastPeers(server.Id));
                    }

                    if (health.Reachable)
                        _lastPeersCount[server.Id] = health.PeersActive;
                    else
                        health = health with { PeersActive = GetLastPeers(server.Id) };

                    await mediator.Send(
                        new EvaluateServerHealthRequest(
                            server.Id,
                            health.Reachable,
                            health.PeersActive
                        ),
                        stoppingToken
                    );
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking servers health");
                Console.WriteLine($"[ServerHealth] ERROR: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private int GetLastPeers(Guid serverId) =>
        _lastPeersCount.TryGetValue(serverId, out var peers) ? peers : 0;
}
