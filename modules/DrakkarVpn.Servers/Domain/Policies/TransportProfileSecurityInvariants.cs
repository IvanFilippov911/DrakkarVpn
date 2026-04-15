using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Servers.Domain.Policies;

/// <summary>
/// Central switch for <see cref="SecurityType"/> rules on <c>ServerTransportProfile</c>.
/// When adding a new <see cref="SecurityType"/> value, extend every public method here.
/// </summary>
public static class TransportProfileSecurityInvariants
{
    /// <summary>
    /// Ensures mutating Reality wire parameters is defined for the current security mode.
    /// </summary>
    public static void EnsureRealitySettingsMutationAllowed(SecurityType securityType)
    {
        switch (securityType)
        {
            case SecurityType.Reality:
                return;
            case SecurityType.Tls:
                throw new InvalidOperationException(
                    "Reality transport parameters cannot be updated when security type is Tls.");
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(securityType),
                    securityType,
                    "Unhandled SecurityType: extend TransportProfileSecurityInvariants.EnsureRealitySettingsMutationAllowed.");
        }
    }

    /// <summary>
    /// Validates that a Reality-parameterized factory may target the given security mode.
    /// </summary>
    public static void EnsureRealityFactorySecurity(SecurityType securityType)
    {
        switch (securityType)
        {
            case SecurityType.Reality:
                return;
            case SecurityType.Tls:
                throw new ArgumentException(
                    "Reality transport factories cannot target Tls security mode.",
                    nameof(securityType));
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(securityType),
                    securityType,
                    "Unhandled SecurityType: extend TransportProfileSecurityInvariants.EnsureRealityFactorySecurity.");
        }
    }
}
