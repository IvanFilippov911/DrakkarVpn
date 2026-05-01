using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Agent.Infrastructure.EF.Repositories;

public sealed class AgentTransportStateRepository : IAgentTransportStateRepository
{
    private const int SingletonRowId = 1;
    private readonly AgentDbContext _db;

    public AgentTransportStateRepository(AgentDbContext db)
    {
        _db = db;
    }

    public Task<AgentTransportStateDto?> GetCurrentAsync(CancellationToken ct) =>
        _db.AgentTransportStates
            .AsNoTracking()
            .Where(x => x.Id == SingletonRowId)
            .Select(x => new AgentTransportStateDto(
                x.ServerId,
                x.ActivationId,
                x.OperationId,
                x.PayloadHash,
                x.AppliedAtUtc,
                x.UpdatedAtUtc))
            .FirstOrDefaultAsync(ct);

    public async Task UpsertAppliedAsync(
        Guid serverId,
        Guid activationId,
        Guid operationId,
        string payloadHash,
        DateTime appliedAtUtc,
        CancellationToken ct)
    {
        appliedAtUtc = DateTime.SpecifyKind(appliedAtUtc, DateTimeKind.Utc);
        var row = await _db.AgentTransportStates.FirstOrDefaultAsync(x => x.Id == SingletonRowId, ct);

        if (row is null)
            _db.AgentTransportStates.Add(
                AgentTransportState.Create(serverId, activationId, operationId, payloadHash, appliedAtUtc));
        else
            row.MarkApplied(serverId, activationId, operationId, payloadHash, appliedAtUtc);

        await _db.SaveChangesAsync(ct);
    }
}
