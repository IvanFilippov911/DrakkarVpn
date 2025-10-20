using DrakkarVpn.Shared;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IBenchmarkService
{
    Task<BenchmarkResultDto> RunBenchmarkAsync(CancellationToken ct = default);
}