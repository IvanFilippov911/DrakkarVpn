using DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportApplyJob;

public sealed record GetServerTransportApplyJobQuery(Guid ServerId, Guid JobId)
    : IRequest<ServerTransportApplyJobStatusDto?>;
