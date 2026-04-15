namespace NetworkMonitoring.Domain;

public sealed class ProbeNode
{
    public const int NameMaxLength = 200;
    public const int RegionMaxLength = 64;
    public const int HostMaxLength = 255;

    private ProbeNode() { }

    private ProbeNode(
        Guid id,
        string name,
        string region,
        string host,
        ProbeNodeStatus status,
        bool isEnabled,
        DateTime? lastSeenAtUtc,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Probe node id must not be empty.", nameof(id));

        Id = id;
        Name = NormalizeRequired(name, nameof(name), NameMaxLength);
        Region = NormalizeRequired(region, nameof(region), RegionMaxLength);
        Host = NormalizeRequired(host, nameof(host), HostMaxLength);

        Status = status;
        IsEnabled = isEnabled;
        LastSeenAtUtc = lastSeenAtUtc.HasValue
            ? EnsureUtc(lastSeenAtUtc.Value, nameof(lastSeenAtUtc))
            : null;

        CreatedAtUtc = EnsureUtc(createdAtUtc, nameof(createdAtUtc));
        UpdatedAtUtc = EnsureUtc(updatedAtUtc, nameof(updatedAtUtc));
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Region { get; private set; } = default!;
    public string Host { get; private set; } = default!;

    public ProbeNodeStatus Status { get; private set; }
    public bool IsEnabled { get; private set; }

    public DateTime? LastSeenAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static ProbeNode Register(
        Guid id,
        string name,
        string region,
        string host,
        DateTime createdAtUtc)
    {
        createdAtUtc = EnsureUtc(createdAtUtc, nameof(createdAtUtc));

        return new ProbeNode(
            id: id,
            name: name,
            region: region,
            host: host,
            status: ProbeNodeStatus.Healthy,
            isEnabled: true,
            lastSeenAtUtc: null,
            createdAtUtc: createdAtUtc,
            updatedAtUtc: createdAtUtc);
    }

    public void Rename(string name, DateTime utcNow)
    {
        Name = NormalizeRequired(name, nameof(name), NameMaxLength);
        Touch(utcNow);
    }

    public void ChangeRegion(string region, DateTime utcNow)
    {
        Region = NormalizeRequired(region, nameof(region), RegionMaxLength);
        Touch(utcNow);
    }

    public void ChangeHost(string host, DateTime utcNow)
    {
        Host = NormalizeRequired(host, nameof(host), HostMaxLength);
        Touch(utcNow);
    }

    public void MarkSeen(DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow, nameof(utcNow));

        if (!IsEnabled)
            throw new InvalidOperationException("Disabled probe node cannot be marked as seen.");

        LastSeenAtUtc = utcNow;
        Status = ProbeNodeStatus.Healthy;
        UpdatedAtUtc = utcNow;
    }

    public void MarkOffline(DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow, nameof(utcNow));

        if (!IsEnabled)
            return;

        Status = ProbeNodeStatus.Offline;
        UpdatedAtUtc = utcNow;
    }

    public void Disable(DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow, nameof(utcNow));

        IsEnabled = false;
        Status = ProbeNodeStatus.Disabled;
        UpdatedAtUtc = utcNow;
    }

    public void Enable(DateTime utcNow)
    {
        utcNow = EnsureUtc(utcNow, nameof(utcNow));

        IsEnabled = true;
        Status = ProbeNodeStatus.Healthy;
        UpdatedAtUtc = utcNow;
    }

    private void Touch(DateTime utcNow)
    {
        UpdatedAtUtc = EnsureUtc(utcNow, nameof(utcNow));
    }

    private static string NormalizeRequired(string value, string paramName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(paramName);

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