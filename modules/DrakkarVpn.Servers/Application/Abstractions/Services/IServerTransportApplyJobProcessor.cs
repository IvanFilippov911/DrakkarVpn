namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IServerTransportApplyJobProcessor
{
    Task ProcessAsync(Guid jobId, CancellationToken ct);
}