using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record GetServersRequest(string? Region, string? Status) : IRequest<IReadOnlyList<GetServersDto>>;