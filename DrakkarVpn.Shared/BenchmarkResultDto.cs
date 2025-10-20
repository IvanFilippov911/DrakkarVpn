namespace DrakkarVpn.Shared;

public sealed record BenchmarkResultDto(
    double DownloadMbps,
    double UploadMbps,
    DateTimeOffset Timestamp
)
{
    public double MaxThroughput => Math.Min(DownloadMbps, UploadMbps);
};