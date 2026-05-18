using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;

public interface IServerTransportApplyJobOutcomeClassifier
{
    ServerTransportApplyJobOutcomeBuckets Classify(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        IReadOnlyDictionary<Guid, AgentApplyResult> resultsByJobId);
}
