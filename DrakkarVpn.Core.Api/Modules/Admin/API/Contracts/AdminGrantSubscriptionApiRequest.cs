using System.ComponentModel.DataAnnotations;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed class AdminGrantSubscriptionApiRequest
{
    [Required]
    public Guid TariffId { get; init; }

    public bool MarkUserInternal { get; init; }

    public int? DeviceCount { get; init; }
}