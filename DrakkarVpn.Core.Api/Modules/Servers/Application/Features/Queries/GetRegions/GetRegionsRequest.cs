using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Servers;
using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetRegions;

public sealed record GetRegionsRequest() : IRequest<IReadOnlyList<RegionDto>>;