namespace NetworkMonitoring.Infrastructure.EF.Entities;

public sealed class ProbeResult
{
    public const int ErrorCodeMaxLength = 128;
    public const int ErrorMessageMaxLength = 1000;

    private ProbeResult() { }

    public Guid Id { get; private set; }

    public Guid ProbeNodeId { get; private set; }
    public Guid ServerId { get; private set; }
    public Guid ProfileId { get; private set; }

    public bool Success { get; private set; }
    public int? LatencyMs { get; private set; }

    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }

    public DateTime CheckedAtUtc { get; private set; }

    public static ProbeResult CreateSuccess(
        Guid id,
        Guid probeNodeId,
        Guid serverId,
        Guid profileId,
        int latencyMs,
        DateTime checkedAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (probeNodeId == Guid.Empty) throw new ArgumentException("ProbeNodeId must not be empty.", nameof(probeNodeId));
        if (serverId == Guid.Empty) throw new ArgumentException("ServerId must not be empty.", nameof(serverId));
        if (profileId == Guid.Empty) throw new ArgumentException("ProfileId must not be empty.", nameof(profileId));
        if (latencyMs < 0) throw new ArgumentOutOfRangeException(nameof(latencyMs));
        if (checkedAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Timestamp must be UTC.", nameof(checkedAtUtc));

        return new ProbeResult
        {
            Id = id,
            ProbeNodeId = probeNodeId,
            ServerId = serverId,
            ProfileId = profileId,
            Success = true,
            LatencyMs = latencyMs,
            ErrorCode = null,
            ErrorMessage = null,
            CheckedAtUtc = checkedAtUtc
        };
    }

    public static ProbeResult CreateFailure(
        Guid id,
        Guid probeNodeId,
        Guid serverId,
        Guid profileId,
        string? errorCode,
        string? errorMessage,
        DateTime checkedAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must not be empty.", nameof(id));
        if (probeNodeId == Guid.Empty) throw new ArgumentException("ProbeNodeId must not be empty.", nameof(probeNodeId));
        if (serverId == Guid.Empty) throw new ArgumentException("ServerId must not be empty.", nameof(serverId));
        if (profileId == Guid.Empty) throw new ArgumentException("ProfileId must not be empty.", nameof(profileId));
        if (checkedAtUtc.Kind != DateTimeKind.Utc) throw new ArgumentException("Timestamp must be UTC.", nameof(checkedAtUtc));

        var normalizedCode = NormalizeOptional(errorCode, ErrorCodeMaxLength);
        var normalizedMessage = NormalizeOptional(errorMessage, ErrorMessageMaxLength);

        return new ProbeResult
        {
            Id = id,
            ProbeNodeId = probeNodeId,
            ServerId = serverId,
            ProfileId = profileId,
            Success = false,
            LatencyMs = null,
            ErrorCode = normalizedCode,
            ErrorMessage = normalizedMessage,
            CheckedAtUtc = checkedAtUtc
        };
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(nameof(value), $"Maximum length is {maxLength}.");

        return normalized;
    }
}