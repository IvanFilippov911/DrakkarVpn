using MediatR;

namespace DrakkarVpn.Servers.Application.Handlers.RunServerTransportApplyJob;

public sealed record RunServerTransportApplyJobRequest(Guid JobId) : IRequest<Unit>;