using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.ChangeServerStatus;
public sealed record ChangeServerStatusRequest(Guid ServerId, ServerStatus Status) : IRequest<bool>;