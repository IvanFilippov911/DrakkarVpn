using DrakkarVpn.Servers.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Servers.Application.Handlers.RunServerTransportApplyJob;

public sealed class RunServerTransportApplyJobHandler
    : IRequestHandler<RunServerTransportApplyJobRequest, Unit>
{
    private readonly IServerTransportApplyJobProcessor _processor;

    public RunServerTransportApplyJobHandler(IServerTransportApplyJobProcessor processor)
    {
        _processor = processor;
    }

    public async Task<Unit> Handle(RunServerTransportApplyJobRequest request, CancellationToken ct)
    {
        await _processor.ProcessAsync(request.JobId, ct);
        return Unit.Value;
    }
}