using System.Runtime.CompilerServices;

namespace DrakkarVpn.Servers.Application.Common.Guards;

internal static class LeaseOwnerGuard
{
    public const int MaxLength = 128;

    public static string Require(
        string? value,
        int maxLength = MaxLength,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("leaseOwner is required.", paramName);

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"leaseOwner must be at most {maxLength} characters.", paramName);

        return trimmed;
    }
}
