namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record HomeContextDto(
    HomeScreenStateDto State,
    Guid? PendingProvisionJobId,
    string? PollUrl);
