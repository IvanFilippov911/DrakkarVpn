using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Options;
using DrakkarVpn.Peers.Application.Abstractions.Services;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Peers.Application.Services.Provisioning;

public sealed class PeerProvisionPayloadFactory : IPeerProvisionPayloadFactory
{
    private readonly VlessLinkOptions _opt;

    public PeerProvisionPayloadFactory(IOptions<VlessLinkOptions> opt)
        => _opt = opt.Value;

    public PeerProvisionPayloadDto CreateNew(string publicHost)
        => Create(Guid.NewGuid(),  publicHost);

    public PeerProvisionPayloadDto Create(Guid agentPeerUuid, string publicHost)
    {
        if (agentPeerUuid == Guid.Empty)
            throw new ArgumentException("agentPeerUuid is required", nameof(agentPeerUuid));

        if (string.IsNullOrWhiteSpace(publicHost))
            throw new InvalidOperationException("PublicHost is required");
        
        var port = _opt.PublicPort;
        
        var vlessUrl =
            $"vless://{agentPeerUuid}@{publicHost}:{port}" +
            $"?security=tls&encryption=none&type=tcp" +
            (_opt.AllowInsecure ? "&allowInsecure=1" : "") +
            $"#{_opt.TagPrefix}-{agentPeerUuid.ToString()[..8]}";

        return new PeerProvisionPayloadDto(agentPeerUuid, vlessUrl);
    }
}