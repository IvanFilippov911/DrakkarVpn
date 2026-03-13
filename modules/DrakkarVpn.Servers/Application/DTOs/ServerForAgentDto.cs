namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed class ServerForAgentDto
{
    public Guid Id { get; }
    public Uri AgentBaseUrl { get; }

    public ServerForAgentDto(Guid id, Uri agentBaseUrl)
    {
        Id = id;
        AgentBaseUrl = agentBaseUrl ?? throw new ArgumentNullException(nameof(agentBaseUrl));
    }
}