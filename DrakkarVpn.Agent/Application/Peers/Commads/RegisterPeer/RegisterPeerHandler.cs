using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RegisterPeer;

public sealed class RegisterPeerHandler 
    : IRequestHandler<RegisterPeerCommand, RegisterPeerResponseDto>
{
    private readonly IXrayPeerClient _v2ray;

    public RegisterPeerHandler(IXrayPeerClient v2ray)
    {
        _v2ray = v2ray;
    }

    public Task<RegisterPeerResponseDto> Handle(RegisterPeerCommand request, CancellationToken ct)
        => _v2ray.RegisterPeerAsync(ct);
}