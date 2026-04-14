namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record ServerConfigDataDto(
    string Region,
    string PublicHost,
    int    PublicPort);

