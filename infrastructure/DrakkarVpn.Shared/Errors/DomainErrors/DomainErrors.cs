namespace DrakkarVpn.Shared.Errors.DomainErrors;

public static class DomainErrors
{
    public static class Tariff
    {
        public static readonly Error InvalidName =
            new(
                DomainArea.Tariff,
                "Tariff.InvalidName",
                "Tariff name must be between 3 and 50 characters.",
                DomainErrorType.Info
            );

        public static readonly Error InvalidDuration =
            new(
                DomainArea.Tariff,
                "Tariff.InvalidDuration",
                "Tariff duration must be positive and not exceed 5 years.",
                DomainErrorType.Info
            );

        public static readonly Error InvalidPrice =
            new(
                DomainArea.Tariff,
                "Tariff.InvalidPrice",
                "Tariff price must be greater than zero and not exceed 10 000.",
                DomainErrorType.Info
            );

        public static readonly Error InvalidMaxDevices =
            new(
                DomainArea.Tariff,
                "Tariff.InvalidMaxDevices",
                "Tariff max devices must be between 1 and 50.",
                DomainErrorType.Info
            );
    }

    // На будущее:
    // public static class User { ... }
    // public static class Subscription { ... }
    // public static class Peers { ... }
}