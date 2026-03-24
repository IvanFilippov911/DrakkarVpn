using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;

public sealed record GetPeerByUuidQuery(Guid PeerUuid) : IRequest<PeerConfigDto?>;