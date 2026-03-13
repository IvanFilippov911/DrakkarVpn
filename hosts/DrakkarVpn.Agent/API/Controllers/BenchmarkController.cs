using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Benchmark.Command.RunBenchmark;
using DrakkarVpn.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.Controllers;

[ApiController]
[Route("benchmark")]
public class BenchmarkController : ControllerBase
{
    private readonly IMediator _mediator;
    public BenchmarkController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<BenchmarkResultDto>> Run(CancellationToken ct)
    {
        var result = await _mediator.Send(new RunBenchmarkCommand(), ct);
        return Ok(result);
    }
}