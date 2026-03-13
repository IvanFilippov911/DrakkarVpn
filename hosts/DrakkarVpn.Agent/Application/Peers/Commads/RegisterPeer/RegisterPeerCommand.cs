using DrakkarVpn.Agent.Application.DTOs;
using MediatR;

using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RegisterPeer;

public sealed record RegisterPeerCommand(Guid PeerUuid) : IRequest;