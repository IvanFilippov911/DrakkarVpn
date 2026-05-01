using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.Exception;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RegisterPeer;

public sealed class RegisterPeerHandler
    : IRequestHandler<RegisterPeerCommand>
{
    private readonly IXrayPeerClient _xray;

    public RegisterPeerHandler(IXrayPeerClient xray)
        => _xray = xray;

    public async Task Handle(RegisterPeerCommand cmd, CancellationToken ct)
    {
        try
        {
            await _xray.RegisterPeerAsync(cmd.PeerUuid, ct);
        }
        catch (PeersAgentAlreadyExistsException)
        {
            return;
        }
    }
}