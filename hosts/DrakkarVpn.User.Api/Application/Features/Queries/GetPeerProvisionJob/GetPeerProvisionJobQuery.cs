using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerProvisionJob;

public sealed record GetPeerProvisionJobQuery(Guid JobId) : IRequest<PeerProvisionJobResponse?>;