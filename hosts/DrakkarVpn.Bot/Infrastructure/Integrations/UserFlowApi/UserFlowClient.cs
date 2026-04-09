using System.Net.Http.Json;
using DrakkarVpn.Bot.Application.Abstractions;
using DrakkarVpn.Bot.Infrastructure.Integrations.UserFlowApi.Exceptions;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Bot.Infrastructure.Integrations.UserFlowApi;

public sealed class UserFlowClient : IUserFlowClient
{
    private readonly HttpClient _http;
    private readonly ILogger<UserFlowClient> _logger;

    public UserFlowClient(HttpClient http, ILogger<UserFlowClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task RegisterAsync(long telegramId, string? username, CancellationToken ct)
    {
        var request = new
        {
            TelegramId = telegramId,
            Username = username,
        };

        var response = await _http.PostAsJsonAsync(
            "/api/v1/user/register",
            request,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "User registration failed. TelegramId={TelegramId}, Status={Status}",
                telegramId,
                response.StatusCode);

            throw new UserFlowApiException("Failed to register user");
        }
    }
}