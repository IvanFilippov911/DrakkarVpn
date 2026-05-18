namespace DrakkarVpn.Servers.Application.Options;

public sealed class ServerTransportApplyJobOptions
{
    public const string SectionName = "ServerTransportApplyJob";

    public int DefaultMaxAttempts { get; init; } = 10;
    public int MaxAcquireBatchSize { get; init; } = 500;
    public int MaxErrorCodeLength { get; init; } = 128;
    public int MaxErrorMessageLength { get; init; } = 2048;
    public TimeSpan MinLeaseDuration { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan MaxLeaseDuration { get; init; } = TimeSpan.FromMinutes(5);

    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(2);
    public int BatchSize { get; init; } = 20;
    public TimeSpan LeaseDuration { get; init; } = TimeSpan.FromSeconds(30);
    public TimeSpan ErrorBackoff { get; init; } = TimeSpan.FromSeconds(1);
    public int StartupJitterMs { get; init; } = 200;
}
