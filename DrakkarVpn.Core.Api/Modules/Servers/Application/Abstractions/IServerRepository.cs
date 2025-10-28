using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Models;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerRepository
{
    Task<Server?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(Server server, CancellationToken ct);
    IQueryable<Server> Query();
    Task DeleteAsync(Server server, CancellationToken ct);
    Task UpdateBenchmarkAsync(Guid id, BenchmarkResultDto dto, CancellationToken ct);
    Task<IReadOnlyList<ServerForHealthPoll>> GetForHealthPollAsync(CancellationToken ct);
}