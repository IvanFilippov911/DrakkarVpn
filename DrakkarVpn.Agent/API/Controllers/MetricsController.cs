using DrakkarVpn.Agent.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController : ControllerBase
{
    private readonly IHealthService _healthService;

    public MetricsController(IHealthService healthService)
    {
        _healthService = healthService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _healthService.GetHealthAsync(ct);
        return Ok(result);
    }
}