using System.ComponentModel.DataAnnotations;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;

public sealed class AdminGrantSubscriptionRequest
{
    [Required]
    public Guid TariffId { get; init; }
    
    public int? DeviceCount { get; init; }
}