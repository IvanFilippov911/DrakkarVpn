using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersTrafficSummary;

public sealed record GetServersTrafficSummaryRequest(
    Guid[] ServerIds,
    DateTime FromUtc
) : IRequest<Dictionary<Guid, long>>;