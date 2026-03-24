using System.Threading;
using System.Threading.Tasks;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

public interface IVpnAccessContextService
{
    Task<VpnAccessContextDto> GetVpnAccessContextAsync(
        long telegramId,
        string deviceId,
        CancellationToken ct);
}

