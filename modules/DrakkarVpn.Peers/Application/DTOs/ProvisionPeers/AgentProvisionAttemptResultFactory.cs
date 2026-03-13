namespace DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;

public static class AgentProvisionAttemptResultFactory
{
    public static AgentProvisionAttemptResult Applied(Guid jobId, DateTime nowUtc)
        => new(
            JobId: jobId,
            Applied: true,
            ErrorCode: null,
            ErrorMessage: null,
            AttemptedAtUtc: EnsureUtc(nowUtc));

    public static AgentProvisionAttemptResult Fail(Guid jobId, string errorCode, string? errorMessage, DateTime nowUtc)
        => new(
            JobId: jobId,
            Applied: false,
            ErrorCode: errorCode,
            ErrorMessage: errorMessage,
            AttemptedAtUtc: EnsureUtc(nowUtc));

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}