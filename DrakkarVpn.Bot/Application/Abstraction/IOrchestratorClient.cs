using DrakkarVpn.Bot.Application.DTOs;
using DrakkarVpn.Shared.Servers;

namespace DrakkarVpn.Bot.Infrastructure.Telegram.Abstraction;

public interface IOrchestratorClient
{
    Task<RegisterUserResponse> RegisterUserAsync(long telegramId, CancellationToken ct);
    
    Task<IReadOnlyList<RegionDto>> GetRegionsAsync(CancellationToken ct);
    Task<PeerRegisterResponseDto> AllocatePeerAsync(long TelegramId, string? Region, CancellationToken ct);

    Task<IReadOnlyList<GetTgPeersDto>> GetPeersAsync(long telegramId, CancellationToken ct);
}