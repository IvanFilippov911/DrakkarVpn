namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record GetServersForHealthPollDto(Guid Id, Uri AgentBaseUrl);