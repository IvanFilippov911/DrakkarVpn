namespace DrakkarVpn.Admin.Api.API.Contracts.NetworkMonitoring;

public sealed record RegisterProbeNodeApiRequest(
    string Name,
    string Region,
    string Host);

