using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Peers.Application.Abstractions.Services;

public interface IPeerProvisionPayloadFactory
{
    PeerProvisionPayloadDto CreateNew(string publicHost);
    PeerProvisionPayloadDto Create(Guid agentPeerUuid, string publicHost);
}