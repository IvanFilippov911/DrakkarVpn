using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersForHealthPoll;

public sealed record GetServersForHealthPollRequest() : IRequest<IReadOnlyList<GetServersForHealthPollDto>>;