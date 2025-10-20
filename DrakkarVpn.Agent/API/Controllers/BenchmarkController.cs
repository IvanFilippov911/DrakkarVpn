using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.Controllers;

[ApiController]
[Route("benchmark")]
public class BenchmarkController : ControllerBase
{
    private readonly IBenchmarkService _benchmark;

    public BenchmarkController(IBenchmarkService benchmark)
        => _benchmark = benchmark;

    [HttpPost]
    public async Task<ActionResult<BenchmarkResultDto>> Run(CancellationToken ct)
    {
        var result = await _benchmark.RunBenchmarkAsync(ct);
        return Ok(result);
    }
}