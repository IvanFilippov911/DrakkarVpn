using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.DeleteServer;

public sealed record DeleteServerRequest(Guid ServerId) : IRequest<bool>;