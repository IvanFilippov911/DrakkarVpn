namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public enum AgentApplyOutcome
{
    Applied,
    RetryableFailed,
    PermanentFailed
}