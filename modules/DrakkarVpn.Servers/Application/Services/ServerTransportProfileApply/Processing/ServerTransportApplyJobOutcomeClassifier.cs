using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;

public sealed class ServerTransportApplyJobOutcomeClassifier : IServerTransportApplyJobOutcomeClassifier
{
    private const string ErrorMissingResult = "agent_apply_missing_result";
    private const string MissingResultMessage = "Agent apply service did not produce result for job.";

    public ServerTransportApplyJobOutcomeBuckets Classify(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        IReadOnlyDictionary<Guid, AgentApplyResult> resultsByJobId)
    {
        var applied = new List<AppliedJobOutcome>();
        var retryable = new List<ServerTransportApplyJobFailure>();
        var permanent = new List<ServerTransportApplyJobFailure>();

        foreach (var job in jobs)
        {
            if (!resultsByJobId.TryGetValue(job.JobId, out var result))
            {
                retryable.Add(new ServerTransportApplyJobFailure(
                    job,
                    ErrorMissingResult,
                    MissingResultMessage));
                continue;
            }

            switch (result.Outcome)
            {
                case AgentApplyOutcome.Applied:
                    applied.Add(new AppliedJobOutcome(
                        job.JobId,
                        job.ServerId,
                        job.ActivationId,
                        job.TargetTransportVersion));
                    break;

                case AgentApplyOutcome.PermanentFailed:
                    permanent.Add(new ServerTransportApplyJobFailure(
                        job,
                        result.ErrorCode ?? ErrorMissingResult,
                        result.ErrorMessage));
                    break;

                case AgentApplyOutcome.RetryableFailed:
                default:
                    retryable.Add(new ServerTransportApplyJobFailure(
                        job,
                        result.ErrorCode ?? ErrorMissingResult,
                        result.ErrorMessage));
                    break;
            }
        }

        return new ServerTransportApplyJobOutcomeBuckets(applied, retryable, permanent);
    }
}
