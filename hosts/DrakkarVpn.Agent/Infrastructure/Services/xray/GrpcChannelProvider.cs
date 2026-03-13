using System.Collections.Concurrent;
using Grpc.Net.Client;

namespace DrakkarVpn.Agent.Infrastructure.Services.Grpc;

public sealed class GrpcChannelProvider : IGrpcChannelProvider, IDisposable
{
    private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private bool _disposed;

    public GrpcChannelProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public GrpcChannel GetChannel(string baseAddress)
    {
        if (string.IsNullOrWhiteSpace(baseAddress)) throw new ArgumentNullException(nameof(baseAddress));
        
        var key = baseAddress.TrimEnd('/');

        return _channels.GetOrAdd(key, k =>
        {
            var httpClient = _httpClientFactory.CreateClient("grpc");

            var options = new GrpcChannelOptions
            {
                HttpClient = httpClient
            };

            return GrpcChannel.ForAddress(k, options);
        });
    }

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var ch in _channels.Values)
        {
            try { ch.Dispose(); } catch { }
        }
        _channels.Clear();
        _disposed = true;
    }
}