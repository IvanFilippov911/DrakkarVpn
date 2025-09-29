using System.Net.Http.Json;
using DrakkarVpn.Bot.Application.DTOs;
using DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;
using DrakkarVpn.Bot.Infrastructure.Telegram.Exceptions;
using DrakkarVpn.Bot.Infrastructure.Telegram.Options;
using DrakkarVpn.Shared.Servers;
using DrakkarVpn.Shared.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Bot.Infrastructure.Telegram;

public class OrchestratorClient : IOrchestratorClient
{
    private readonly HttpClient _http;
    private readonly ILogger<OrchestratorClient> _logger;
    private readonly OrchestratorOptions _options;

    public OrchestratorClient(HttpClient http, ILogger<OrchestratorClient> logger, IOptions<OrchestratorOptions> options)
    {
        _http = http;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<RegisterUserResponse> RegisterUserAsync(long telegramId, CancellationToken ct)
    {
        var resp = await _http.PostAsJsonAsync(
            _options.RegisterUserEndpoint,
            new RegisterUserRequest(telegramId), ct);

        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to register user {TgId}, Status {Status}", telegramId, resp.StatusCode);
            throw new OrchestratorException("Failed to register user");
        }
        
        var result = await resp.Content.ReadFromJsonAsync<RegisterUserResponse>(cancellationToken: ct);
        if (result is null)
            throw new OrchestratorException("Register user response deserialization failed");

        return result;
    }

    public async Task<IReadOnlyList<RegionDto>> GetRegionsAsync(CancellationToken ct)
    {
        var resp = await _http.GetAsync(_options.RegionsEndpoint, ct);
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch regions, Status {Status}", resp.StatusCode);
            throw new OrchestratorException("Failed to fetch regions");
        }

        return (await resp.Content.ReadFromJsonAsync<List<RegionDto>>(cancellationToken: ct))
               ?? new List<RegionDto>();
    }

    public async Task<PeerRegisterResponseDto> AllocatePeerAsync(long TelegramId, string? Region, CancellationToken ct)
    {
        var resp = await _http.PostAsJsonAsync(
            _options.AllocatePeerEndpoint,
            new { TelegramId = TelegramId, Region = Region }, ct);

        if (!resp.IsSuccessStatusCode)
            throw new OrchestratorException("Failed to create peer");

        var peer = await resp.Content.ReadFromJsonAsync<PeerRegisterResponseDto>(cancellationToken: ct);
        if (peer is null)
            throw new OrchestratorException("Failed to deserialize peer response");

        return peer;
    }
    
    public async Task<IReadOnlyList<GetTgPeersDto>> GetPeersAsync(long telegramId, CancellationToken ct)
    {
        var resp = await _http.GetAsync($"{_options.GetPeersEndpoint}?telegramId={telegramId}", ct);

        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch peers for {TgId}, Status {Status}", telegramId, resp.StatusCode);
            throw new OrchestratorException("Failed to fetch peers");
        }

        return (await resp.Content.ReadFromJsonAsync<List<GetTgPeersDto>>(cancellationToken: ct))
               ?? new List<GetTgPeersDto>();
    }

}