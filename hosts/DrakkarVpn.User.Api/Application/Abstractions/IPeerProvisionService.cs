using System.Threading;
using System.Threading.Tasks;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

public interface IPeerProvisionService
{
    Task<Guid> StartVpnConfigProvisioningAsync(
        VpnAccessContextDto access,
        CancellationToken ct);
}


