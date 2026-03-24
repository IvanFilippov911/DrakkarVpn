using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerProvisionJob;

public sealed record GetPeerProvisionJobQuery(Guid JobId) : IRequest<PeerProvisionJobDto?>;