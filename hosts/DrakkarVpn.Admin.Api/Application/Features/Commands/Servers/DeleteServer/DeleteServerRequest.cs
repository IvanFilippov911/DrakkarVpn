using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.DeleteServer;

public sealed record DeleteServerRequest(Guid ServerId)
    : IRequest<bool>, IServersCommand<bool>;