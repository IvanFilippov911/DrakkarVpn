using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IVpnConfigBuilder
{
    string Build(
        PeerDataForConfigDto peer,
        ServerConfigDataDto server);
}