using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Domain.Validation;

internal static class TariffValidation
{
    internal static void ValidateAndThrow(
        string name,
        TimeSpan duration,
        decimal price,
        int defaultMaxDevices)
    {
        ValidateName(name);
        ValidateDuration(duration);
        ValidatePrice(price);
        ValidateMaxDevices(defaultMaxDevices);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length is < 3 or > 50)
            throw DomainErrors.Tariff.InvalidName.ToException();
    }

    private static void ValidateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero ||
            duration > TimeSpan.FromDays(365 * 5))
            throw DomainErrors.Tariff.InvalidDuration.ToException();
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0 || price > 10_000m)
            throw DomainErrors.Tariff.InvalidPrice.ToException();
    }

    private static void ValidateMaxDevices(int maxDevices)
    {
        if (maxDevices <= 0 || maxDevices > 50)
            throw DomainErrors.Tariff.InvalidMaxDevices.ToException();
    }
}