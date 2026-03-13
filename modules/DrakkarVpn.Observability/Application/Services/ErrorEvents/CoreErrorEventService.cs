using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Infrastructure.Entities;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using DrakkarVpn.Observability.Application.Commands;

namespace DrakkarVpn.Observability.Application.Features.Services.ErrorEvents;

public sealed class CoreErrorEventService : ICoreErrorEventService
{
    private readonly ICoreErrorEventRepository _repo;

    public CoreErrorEventService(ICoreErrorEventRepository repo) => _repo = repo;

    public async Task LogAsync(LogCoreErrorEventArgs a, CancellationToken ct)
    {
        var evt = new CoreErrorEvent
        {
            Id           = Guid.NewGuid(),
            TimestampUtc = DateTime.UtcNow,

            Command    = a.Command,
            Area       = a.Area,
            ErrorType  = a.ErrorType,
            DomainCode = a.DomainCode,

            Message = a.Message,
            TraceId = a.TraceId,

            UserId     = a.UserId,
            TelegramId = a.TelegramId,
            PayloadJson = a.PayloadJson
        };

        await _repo.AddAsync(evt, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => _repo.DeleteAsync(id, ct);
}