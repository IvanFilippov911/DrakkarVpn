using System.Threading;
using System.Threading.Tasks;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

public interface ICurrentVpnConfigService
{
    Task<CurrentVpnConfigDto?> GetCurrentConfigAsync(
        VpnAccessContextDto access,
        CancellationToken ct);
}

