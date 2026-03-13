using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Options;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerProvisionPayloadFactory : IPeerProvisionPayloadFactory
{
    private readonly VlessLinkOptions _opt;

    public PeerProvisionPayloadFactory(IOptions<VlessLinkOptions> opt)
        => _opt = opt.Value;

    public PeerProvisionPayloadDto CreateNew()
        => Create(Guid.NewGuid());

    public PeerProvisionPayloadDto Create(Guid agentPeerUuid)
    {
        if (agentPeerUuid == Guid.Empty)
            throw new ArgumentException("agentPeerUuid is required", nameof(agentPeerUuid));

        if (string.IsNullOrWhiteSpace(_opt.PublicHost))
            throw new InvalidOperationException("VlessLinkOptions.PublicHost is required");

        var host = _opt.PublicHost.Trim();
        var port = _opt.PublicPort;
        
        var vlessUrl =
            $"vless://{agentPeerUuid}@{host}:{port}" +
            $"?security=tls&encryption=none&type=tcp" +
            (_opt.AllowInsecure ? "&allowInsecure=1" : "") +
            $"#{_opt.TagPrefix}-{agentPeerUuid.ToString()[..8]}";

        return new PeerProvisionPayloadDto(agentPeerUuid, vlessUrl);
    }
}