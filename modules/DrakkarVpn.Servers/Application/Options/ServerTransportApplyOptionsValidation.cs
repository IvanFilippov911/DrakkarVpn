namespace DrakkarVpn.Servers.Application.Options;

internal static class ServerTransportApplyOptionsValidation
{
    public static bool TryValidateJobOptions(
        ServerTransportApplyJobOptions options,
        out string failureMessage)
    {
        if (options.DefaultMaxAttempts <= 0)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.DefaultMaxAttempts)} must be > 0.";
            return false;
        }

        if (options.MaxAcquireBatchSize <= 0)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.MaxAcquireBatchSize)} must be > 0.";
            return false;
        }

        if (options.BatchSize <= 0)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.BatchSize)} must be > 0.";
            return false;
        }

        if (options.BatchSize > options.MaxAcquireBatchSize)
        {
            failureMessage =
                $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.BatchSize)} must be <= {nameof(options.MaxAcquireBatchSize)}.";
            return false;
        }

        if (options.MaxErrorCodeLength <= 0)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.MaxErrorCodeLength)} must be > 0.";
            return false;
        }

        if (options.MaxErrorMessageLength <= 0)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.MaxErrorMessageLength)} must be > 0.";
            return false;
        }

        if (options.MinLeaseDuration <= TimeSpan.Zero)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.MinLeaseDuration)} must be > 0.";
            return false;
        }

        if (options.MaxLeaseDuration < options.MinLeaseDuration)
        {
            failureMessage =
                $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.MaxLeaseDuration)} must be >= {nameof(options.MinLeaseDuration)}.";
            return false;
        }

        if (options.LeaseDuration < options.MinLeaseDuration || options.LeaseDuration > options.MaxLeaseDuration)
        {
            failureMessage =
                $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.LeaseDuration)} must be between {nameof(options.MinLeaseDuration)} and {nameof(options.MaxLeaseDuration)}.";
            return false;
        }

        if (options.Interval <= TimeSpan.Zero)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.Interval)} must be > 0.";
            return false;
        }

        if (options.ErrorBackoff < TimeSpan.Zero)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.ErrorBackoff)} must be >= 0.";
            return false;
        }

        if (options.StartupJitterMs < 0)
        {
            failureMessage = $"{ServerTransportApplyJobOptions.SectionName}:{nameof(options.StartupJitterMs)} must be >= 0.";
            return false;
        }

        failureMessage = string.Empty;
        return true;
    }

    public static bool TryValidateAgentOptions(
        ServerTransportAgentApplyOptions options,
        out string failureMessage)
    {
        if (options.MaxParallelRequests <= 0)
        {
            failureMessage = $"{ServerTransportAgentApplyOptions.SectionName}:{nameof(options.MaxParallelRequests)} must be > 0.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.InboundTag))
        {
            failureMessage = $"{ServerTransportAgentApplyOptions.SectionName}:{nameof(options.InboundTag)} is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Flow))
        {
            failureMessage = $"{ServerTransportAgentApplyOptions.SectionName}:{nameof(options.Flow)} is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Encryption))
        {
            failureMessage = $"{ServerTransportAgentApplyOptions.SectionName}:{nameof(options.Encryption)} is required.";
            return false;
        }

        failureMessage = string.Empty;
        return true;
    }

    public static bool TryValidateCombined(
        ServerTransportApplyJobOptions jobOptions,
        ServerTransportAgentApplyOptions agentOptions,
        out string failureMessage)
    {
        if (agentOptions.MaxParallelRequests < jobOptions.BatchSize)
        {
            failureMessage =
                $"{ServerTransportAgentApplyOptions.SectionName}:{nameof(agentOptions.MaxParallelRequests)} must be >= " +
                $"{ServerTransportApplyJobOptions.SectionName}:{nameof(jobOptions.BatchSize)}.";
            return false;
        }

        failureMessage = string.Empty;
        return true;
    }
}
