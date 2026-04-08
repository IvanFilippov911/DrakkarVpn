namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

/// <summary>
/// Grants a trial subscription to a newly registered user when eligible (no prior trial marker).
/// </summary>
public interface IGrantTrialSubscriptionService
{
    Task TryGrantForNewUserAsync(Guid userId, DateTime nowUtc, CancellationToken ct);
}
