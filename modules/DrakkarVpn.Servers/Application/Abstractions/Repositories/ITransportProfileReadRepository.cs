using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface ITransportProfileReadRepository
{
    Task<(IReadOnlyList<TransportProfileDto> Items, int Total)> GetPagedAsync(
        GetTransportProfilesFilterDto filter,
        CancellationToken ct);
}
