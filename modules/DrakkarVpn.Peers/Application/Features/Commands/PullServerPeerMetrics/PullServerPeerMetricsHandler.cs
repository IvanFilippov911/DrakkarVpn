using System.Net.Http.Json;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.UpsertPeerMetrics;
using DrakkarVpn.Shared.Peers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.PullServerPeerMetrics;
/*
public sealed class PullServerPeerMetricsHandler
    : IRequestHandler<PullServerPeerMetricsCommand, Unit>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediator _mediator;
    private readonly ILogger<PullServerPeerMetricsHandler> _logger;

    public PullServerPeerMetricsHandler(
        IHttpClientFactory httpClientFactory,
        IMediator mediator,
        ILogger<PullServerPeerMetricsHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task<Unit> Handle(PullServerPeerMetricsCommand cmd, CancellationToken ct)
    {
        var server = await _mediator.Send(new GetServerForAgentByIdRequest(cmd.ServerId), ct);

        if (server is null)
        {
            _logger.LogWarning("Server {ServerId} not found for peer metrics poll", cmd.ServerId);
            return Unit.Value;
        }

        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(5);

        PeerMetricsDto[]? peersFromAgent;
        try
        {
            peersFromAgent = await http.GetFromJsonAsync<PeerMetricsDto[]>(
                $"{server.AgentBaseUrl}metrics/peers", ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to poll peers from agent {Url}", server.AgentBaseUrl);
            return Unit.Value;
        }

        peersFromAgent ??= Array.Empty<PeerMetricsDto>();
        
        await _mediator.Send(
            new UpsertPeerMetricsCommand(cmd.ServerId, peersFromAgent),
            ct);

        return Unit.Value;
    }
}

*/