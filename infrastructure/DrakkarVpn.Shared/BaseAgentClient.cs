using System.Net.Http.Json;

namespace DrakkarVpn.Shared;

public abstract class BaseAgentClient
{
    protected readonly HttpClient _http;
    protected BaseAgentClient(HttpClient http) => _http = http;

    protected async Task<T> GetAsync<T>(string url, CancellationToken ct)
    {
        var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    protected async Task PostAsync<T>(string url, T body, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync(url, body, ct);
        response.EnsureSuccessStatusCode();
    }
}