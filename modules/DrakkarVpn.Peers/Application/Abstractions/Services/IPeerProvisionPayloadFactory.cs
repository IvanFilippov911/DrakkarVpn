using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerProvisionPayloadFactory
{
    PeerProvisionPayloadDto Create(Guid agentPeerUuid);
    PeerProvisionPayloadDto CreateNew();
}