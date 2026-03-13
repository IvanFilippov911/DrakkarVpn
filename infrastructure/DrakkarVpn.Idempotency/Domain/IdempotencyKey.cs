namespace DrakkarVpn.Core.Api.Modules.Idempotency.Domain;

public sealed class IdempotencyKey
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string ActorKey { get; private set; } = default!; 
    public string Action { get; private set; } = default!;   
    public Guid RequestId { get; private set; }             

    public IdempotencyStatus Status { get; private set; }
    public string? ResultJson { get; private set; }        
    public string? Error { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }         

    private IdempotencyKey() { }
    private IdempotencyKey(string actorKey, string action, Guid requestId, DateTime nowUtc, TimeSpan? ttl)
    {
        ActorKey = actorKey;
        Action = action;
        RequestId = requestId;
        Status = IdempotencyStatus.Started;
        CreatedAt = nowUtc;
        ExpiresAt = ttl is null ? null : nowUtc.Add(ttl.Value);
    }

    public static IdempotencyKey Start(string actorKey, string action, Guid requestId, DateTime nowUtc, TimeSpan? ttl = null)
        => new(actorKey, action, requestId, nowUtc, ttl);

    public void Succeed(string? resultJson, DateTime nowUtc)
    {
        Status = IdempotencyStatus.Succeeded;
        ResultJson = resultJson;
        CompletedAt = nowUtc;
    }

    public void Fail(string error, DateTime nowUtc)
    {
        Status = IdempotencyStatus.Failed;
        Error = error;
        CompletedAt = nowUtc;
    }
}