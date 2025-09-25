using DrakkarVpn.Agent.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;

    public HealthController(IHealthService healthService)
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