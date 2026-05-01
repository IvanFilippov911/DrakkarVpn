namespace DrakkarVpn.Agent.Application.Options;

public sealed class XrayApplyOptions
{
    public string ConfigPath { get; init; } = "/etc/xray/config.json";
    public string ContainerName { get; init; } = "xray";
    public TimeSpan RestartTimeout { get; init; } = TimeSpan.FromSeconds(15);
    public int HealthAttempts { get; init; } = 20;
    public TimeSpan HealthDelay { get; init; } = TimeSpan.FromSeconds(1);
}