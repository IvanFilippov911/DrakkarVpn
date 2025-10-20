using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;

public sealed record GetServerByIdRequest(Guid ServerId) : IRequest<GetServersDetailDto?>;