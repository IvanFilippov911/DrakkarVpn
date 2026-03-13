using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;

public sealed class ServerWithRealtimeStatsRow
{
    public required Server Server { get; init; }
    public ServerRealtimeStats? Stats { get; init; }
}