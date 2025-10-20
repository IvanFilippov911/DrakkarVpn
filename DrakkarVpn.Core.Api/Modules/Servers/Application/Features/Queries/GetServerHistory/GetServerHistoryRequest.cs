using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistory;

public sealed record GetServerHistoryRequest(
    Guid ServerId,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IRequest<IReadOnlyList<ServerMetricsHistoryDto>>;