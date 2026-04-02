using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IVpnBuildArtifactsService
{
    string BuildVlessLink(
        PeerDataForConfigDto peer,
        ServerConfigDataDto server);

    string BuildHappLink(Guid peerUuid);
    
    string BuildV2RayTunDeepLink(Guid peerUuid);

    public string BuildXrayClientConfig(
        PeerDataForConfigDto peer,
        ServerConfigDataDto server);
}