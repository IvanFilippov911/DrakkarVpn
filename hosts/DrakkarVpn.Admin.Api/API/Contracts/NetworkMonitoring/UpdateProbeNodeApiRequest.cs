namespace DrakkarVpn.Admin.Api.API.Contracts.NetworkMonitoring;

public sealed record UpdateProbeNodeApiRequest(
    string Name,
    string Region,
    string Host);

