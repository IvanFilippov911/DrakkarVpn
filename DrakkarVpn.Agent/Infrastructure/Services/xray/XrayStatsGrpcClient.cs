using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Infrastructure.Config;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Xray.App.Stats.Command;

namespace DrakkarVpn.Agent.Infrastructure.Services.Grpc;

public sealed class XrayStatsGrpcClient : IXrayStatsClient
{
    private readonly IGrpcChannelProvider _channels;
    private readonly XrayOptions _options;

    public XrayStatsGrpcClient(
        IGrpcChannelProvider channels,
        IOptions<XrayOptions> options)
    {
        _channels = channels;
        _options  = options.Value;
    }

    private GrpcChannel GetChannel()
        => _channels.GetChannel($"http://{_options.LocalApiHost}:{_options.ApiPort}");

    private StatsService.StatsServiceClient Stats()
        => new StatsService.StatsServiceClient(GetChannel());

    public async Task<IReadOnlyList<Stat>> GetUserStatsAsync(CancellationToken ct)
    {
        var resp = await Stats().QueryStatsAsync(new QueryStatsRequest
        {
            Pattern = "user>>>",
            Reset   = false
        }, cancellationToken: ct);

        return resp.Stat;
    }
}