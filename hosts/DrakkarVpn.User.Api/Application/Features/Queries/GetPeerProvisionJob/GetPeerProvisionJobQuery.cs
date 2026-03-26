using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerProvisionJob;

public sealed record GetPeerProvisionJobQuery(Guid JobId) : IRequest<PeerProvisionJobStatusDto?>;