using DrakkarVpn.Shared;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServersAgentClient
{
    Task<BenchmarkResultDto> RunBenchmarkAsync(Guid serverId, CancellationToken ct);
}