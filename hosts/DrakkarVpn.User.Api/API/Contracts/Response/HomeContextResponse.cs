using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Response;

public sealed record HomeContextResponse(
    HomeScreenStateDto State,
    Guid? PendingProvisionJobId,
    string? PollUrl
);

