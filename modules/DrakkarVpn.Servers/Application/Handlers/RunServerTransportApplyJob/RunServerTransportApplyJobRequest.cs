using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using MediatR;

namespace DrakkarVpn.Servers.Application.Handlers.RunServerTransportApplyJob;

public sealed record RunServerTransportApplyJobRequest(IReadOnlyCollection<ServerTransportApplyJobDto> Jobs, string leaseOwner) : IRequest<Unit>;