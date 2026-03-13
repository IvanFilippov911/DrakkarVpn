namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;

public sealed record ApplyPollStateResultDto(
    IReadOnlyList<Guid> AppliedServerIds,
    IReadOnlyList<Guid> SkippedServerIds
);