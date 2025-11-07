using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUsersTrafficSummary;

public sealed class GetUsersTrafficSummaryHandler
    : IRequestHandler<GetUsersTrafficSummaryRequest, Dictionary<Guid, UsersTrafficSummaryDto>>
{
    private readonly IUserTrafficRepository _trafficRepo;

    public GetUsersTrafficSummaryHandler(IUserTrafficRepository trafficRepo)
    {
        _trafficRepo = trafficRepo;
    }

    public async Task<Dictionary<Guid, UsersTrafficSummaryDto>> Handle(
        GetUsersTrafficSummaryRequest req,
        CancellationToken ct)
    {
        if (req.UserIds is null || req.UserIds.Length == 0)
            return new();

        return await _trafficRepo.GetTrafficLast24hAsync(
            req.ServerId,
            req.UserIds,
            req.From24hUtc,
            ct);
    }
}