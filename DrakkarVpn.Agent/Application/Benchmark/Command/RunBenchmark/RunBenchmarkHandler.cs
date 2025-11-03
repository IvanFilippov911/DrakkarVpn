using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Agent.Application.Benchmark.Command.RunBenchmark;

public sealed class RunBenchmarkHandler
    : IRequestHandler<RunBenchmarkCommand, BenchmarkResultDto>
{
    private readonly IBenchmarkService _benchmark;

    public RunBenchmarkHandler(IBenchmarkService benchmark)
        => _benchmark = benchmark;

    public Task<BenchmarkResultDto> Handle(RunBenchmarkCommand request, CancellationToken ct)
        => _benchmark.RunBenchmarkAsync(ct);
}