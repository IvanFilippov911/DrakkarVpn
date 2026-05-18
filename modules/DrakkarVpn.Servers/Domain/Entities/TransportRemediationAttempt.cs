using DrakkarVpn.Servers.Domain.Enums.Incidents;

namespace DrakkarVpn.Servers.Domain.Entities;

public sealed class TransportRemediationAttempt
{
    public const int FailureReasonMaxLength = 1000;

    private TransportRemediationAttempt() { }

    private TransportRemediationAttempt(
        Guid id,
        Guid incidentId,
        Guid profileId,
        DateTime startedAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Attempt id must not be empty.", nameof(id));
        if (incidentId == Guid.Empty)
            throw new ArgumentException("Incident id must not be empty.", nameof(incidentId));
        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile id must not be empty.", nameof(profileId));

        Id = id;
        IncidentId = incidentId;
        ProfileId = profileId;
        Status = TransporRemediationAttemptStatus.Started;
        StartedAtUtc = EnsureUtc(startedAtUtc, nameof(startedAtUtc));
    }

    public Guid Id { get; private set; }
    public Guid IncidentId { get; private set; }
    public Guid ProfileId { get; private set; }

    public TransporRemediationAttemptStatus Status { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTime StartedAtUtc { get; private set; }
    public DateTime? FinishedAtUtc { get; private set; }

    public static TransportRemediationAttempt Start(
        Guid id,
        Guid incidentId,
        Guid profileId,
        DateTime startedAtUtc)
        => new(id, incidentId, profileId, startedAtUtc);

    public void MarkFailed(string? failureReason, DateTime utcNow)
    {
        EnsureStarted();

        Status = TransporRemediationAttemptStatus.Failed;
        FailureReason = NormalizeOptional(failureReason, nameof(failureReason), FailureReasonMaxLength);
        FinishedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
    }

    public void MarkSucceeded(DateTime utcNow)
    {
        EnsureStarted();

        Status = TransporRemediationAttemptStatus.Succeeded;
        FailureReason = null;
        FinishedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
    }

    private void EnsureStarted()
    {
        if (Status != TransporRemediationAttemptStatus.Started)
            throw new InvalidOperationException("Only started attempt can be completed.");
    }

    private static string? NormalizeOptional(string? value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
            throw new ArgumentOutOfRangeException(paramName, $"Maximum length is {maxLength}.");

        return normalized;
    }

    private static DateTime EnsureUtc(DateTime value, string paramName)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Timestamp must be UTC.", paramName);

        return value;
    }
}