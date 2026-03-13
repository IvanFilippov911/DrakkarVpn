namespace DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;

public sealed class CoreErrorEvent
{
    public Guid Id { get; set; }
    public DateTime TimestampUtc { get; set; }

    public string Command { get; set; } = default!;
    public string Area { get; set; } = default!;
    public string ErrorType { get; set; } = default!;
    public string? DomainCode { get; set; }

    public string Message { get; set; } = default!;
    public string TraceId { get; set; } = default!;

    public string? UserId { get; set; }
    public string? TelegramId { get; set; }

    public string? PayloadJson { get; set; }
}