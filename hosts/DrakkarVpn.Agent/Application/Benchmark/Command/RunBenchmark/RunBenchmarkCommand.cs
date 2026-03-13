using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Agent.Application.Benchmark.Command.RunBenchmark;

public sealed record RunBenchmarkCommand() : IRequest<BenchmarkResultDto>;