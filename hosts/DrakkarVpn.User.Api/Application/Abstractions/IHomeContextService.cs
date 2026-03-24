using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;

public interface IHomeContextService
{
    Task<HomeContextDto> GetAsync(
        long telegramId,
        string deviceId,
        CancellationToken ct);
}

