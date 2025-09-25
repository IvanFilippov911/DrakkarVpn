using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.EvaluateServerHealth;

public sealed record EvaluateServerHealthRequest(
    Guid ServerId,
    bool Reachable,
    int PeersActive
) : IRequest<bool>;
