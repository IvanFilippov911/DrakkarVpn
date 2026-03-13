using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerById;

public sealed record GetServerByIdRequest(Guid ServerId)
    : IRequest<GetServersDetailDto?>;