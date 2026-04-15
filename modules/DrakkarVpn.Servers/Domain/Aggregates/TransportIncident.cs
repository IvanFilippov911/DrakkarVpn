using DrakkarVpn.Core.Api.Modules.Servers.Domain.Abstractions;
using DrakkarVpn.Servers.Domain.Enums.Incidents;

namespace DrakkarVpn.Servers.Domain.Aggregates;

public sealed class TransportIncident : IAggregateRoot
{
    public const int LastErrorMaxLength = 1000;
    public const int NotesMaxLength = 1000;

    private readonly List<TransportRemediationAttempt> _attempts = new();

    private TransportIncident() { }

    private TransportIncident(
        Guid id,
        Guid serverId,
        Guid activeProfileId,
        TransportIncidentScope scope,
        TransportIncidentReason reason,
        TransportIncidentTrigger triggeredBy,
        int affectedProbeCount,
        int failedProbeCount,
        DateTime openedAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Incident id must not be empty.", nameof(id));
        if (serverId == Guid.Empty)
            throw new ArgumentException("Server id must not be empty.", nameof(serverId));
        if (activeProfileId == Guid.Empty)
            throw new ArgumentException("Active profile id must not be empty.", nameof(activeProfileId));
        if (affectedProbeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(affectedProbeCount));
        if (failedProbeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(failedProbeCount));
        if (failedProbeCount > affectedProbeCount)
            throw new ArgumentOutOfRangeException(nameof(failedProbeCount), "Failed probes cannot exceed affected probes.");

        Id = id;
        ServerId = serverId;
        ActiveProfileId = activeProfileId;

        Scope = scope;
        Reason = reason;
        TriggeredBy = triggeredBy;

        Status = TransportIncidentStatus.Open;

        AffectedProbeCount = affectedProbeCount;
        FailedProbeCount = failedProbeCount;

        OpenedAtUtc = EnsureUtc(openedAtUtc, nameof(openedAtUtc));
        UpdatedAtUtc = OpenedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid ServerId { get; private set; }
    public Guid ActiveProfileId { get; private set; }

    public TransportIncidentStatus Status { get; private set; }

    public TransportIncidentScope Scope { get; private set; }
    public TransportIncidentReason Reason { get; private set; }
    public TransportIncidentTrigger TriggeredBy { get; private set; }

    public DateTime OpenedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }

    public DateTime? RemediationStartedAtUtc { get; private set; }
    public DateTime? RemediationFinishedAtUtc { get; private set; }
    public DateTime? EscalatedAtUtc { get; private set; }

    public Guid? SuccessfulProfileId { get; private set; }

    public int AffectedProbeCount { get; private set; }
    public int FailedProbeCount { get; private set; }

    public string? LastError { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<TransportRemediationAttempt> Attempts => _attempts;

    public static TransportIncident Open(
        Guid id,
        Guid serverId,
        Guid activeProfileId,
        TransportIncidentScope scope,
        TransportIncidentReason reason,
        TransportIncidentTrigger triggeredBy,
        int affectedProbeCount,
        int failedProbeCount,
        DateTime openedAtUtc)
        => new(
            id,
            serverId,
            activeProfileId,
            scope,
            reason,
            triggeredBy,
            affectedProbeCount,
            failedProbeCount,
            openedAtUtc);

    public void StartRemediation(DateTime utcNow)
    {
        EnsureStatus(TransportIncidentStatus.Open);

        RemediationStartedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
        Status = TransportIncidentStatus.Remediating;
        Touch(utcNow);
    }

    public Guid RegisterAttemptedProfile(Guid profileId, DateTime utcNow)
    {
        EnsureStatus(TransportIncidentStatus.Remediating);

        if (profileId == Guid.Empty)
            throw new ArgumentException("Profile id must not be empty.", nameof(profileId));

        if (_attempts.Any(x => x.ProfileId == profileId))
            throw new InvalidOperationException("This profile has already been attempted in the current incident.");

        var attempt = TransportRemediationAttempt.Start(
            id: Guid.NewGuid(),
            incidentId: Id,
            profileId: profileId,
            startedAtUtc: utcNow);

        _attempts.Add(attempt);
        Touch(utcNow);

        return attempt.Id;
    }

    public void MarkAttemptFailed(Guid attemptId, string? failureReason, DateTime utcNow)
    {
        EnsureStatus(TransportIncidentStatus.Remediating);

        var attempt = FindAttempt(attemptId);
        attempt.MarkFailed(failureReason, utcNow);

        LastError = NormalizeOptional(failureReason, nameof(failureReason), LastErrorMaxLength);
        Touch(utcNow);
    }

    public void RecoverWithProfile(Guid attemptId, Guid successfulProfileId, DateTime utcNow)
    {
        EnsureStatus(TransportIncidentStatus.Remediating);

        if (successfulProfileId == Guid.Empty)
            throw new ArgumentException("Successful profile id must not be empty.", nameof(successfulProfileId));

        var attempt = FindAttempt(attemptId);
        if (attempt.ProfileId != successfulProfileId)
            throw new InvalidOperationException("Successful profile must match remediation attempt profile.");

        attempt.MarkSucceeded(utcNow);

        SuccessfulProfileId = successfulProfileId;
        RemediationFinishedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
        ResolvedAtUtc = RemediationFinishedAtUtc;
        Status = TransportIncidentStatus.Recovered;
        Touch(utcNow);
    }

    public void EscalateToNode(string? lastError, DateTime utcNow)
    {
        EnsureStatus(TransportIncidentStatus.Remediating);

        LastError = NormalizeOptional(lastError, nameof(lastError), LastErrorMaxLength);
        Scope = TransportIncidentScope.Node;
        Reason = TransportIncidentReason.AllStandbyProfilesFailed;
        EscalatedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
        RemediationFinishedAtUtc = EscalatedAtUtc;
        Status = TransportIncidentStatus.Escalated;
        Touch(utcNow);
    }

    public void RecoverAfterEscalation(string? notes, DateTime utcNow)
    {
        EnsureStatus(TransportIncidentStatus.Escalated);

        Notes = NormalizeOptional(notes, nameof(notes), NotesMaxLength);
        ResolvedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
        Status = TransportIncidentStatus.Recovered;
        Touch(utcNow);
    }

    public void UpdateProbeSnapshot(int affectedProbeCount, int failedProbeCount, DateTime utcNow)
    {
        if (affectedProbeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(affectedProbeCount));
        if (failedProbeCount < 0)
            throw new ArgumentOutOfRangeException(nameof(failedProbeCount));
        if (failedProbeCount > affectedProbeCount)
            throw new ArgumentOutOfRangeException(nameof(failedProbeCount), "Failed probes cannot exceed affected probes.");

        AffectedProbeCount = affectedProbeCount;
        FailedProbeCount = failedProbeCount;
        Touch(utcNow);
    }

    public void SetLastError(string? lastError, DateTime utcNow)
    {
        LastError = NormalizeOptional(lastError, nameof(lastError), LastErrorMaxLength);
        Touch(utcNow);
    }

    public void SetNotes(string? notes, DateTime utcNow)
    {
        Notes = NormalizeOptional(notes, nameof(notes), NotesMaxLength);
        Touch(utcNow);
    }

    private TransportRemediationAttempt FindAttempt(Guid attemptId)
    {
        if (attemptId == Guid.Empty)
            throw new ArgumentException("Attempt id must not be empty.", nameof(attemptId));

        return _attempts.FirstOrDefault(x => x.Id == attemptId)
               ?? throw new InvalidOperationException("Transport incident attempt was not found.");
    }

    private void EnsureStatus(TransportIncidentStatus expected)
    {
        if (Status != expected)
            throw new InvalidOperationException(
                $"Operation requires incident status {expected}, current status is {Status}.");
    }

    private void Touch(DateTime utcNow)
    {
        UpdatedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
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