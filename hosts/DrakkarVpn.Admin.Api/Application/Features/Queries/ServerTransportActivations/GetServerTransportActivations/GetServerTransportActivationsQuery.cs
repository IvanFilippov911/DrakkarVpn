using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportActivations;

public sealed record GetServerTransportActivationsQuery(Guid ServerId)
    : IRequest<IReadOnlyList<ServerTransportActivationListItemDto>>;
