using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Core;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class CoreHealthApiMapping
{
    public static CoreHealthApiResponse ToApiResponse(this CoreHealthDto dto)
    {
        return new CoreHealthApiResponse(
            Rps: dto.Rps,
            AvgLatencyMs: dto.AvgLatencyMs,
            ErrorRatePct: dto.ErrorRatePct,
            TotalRequests: dto.TotalRequests,
            TotalErrors: dto.TotalErrors,
            WindowStartUtc: dto.WindowStartUtc,
            WindowEndUtc: dto.WindowEndUtc,
            ErrorsByArea: dto.ErrorsByArea);
    }
}
