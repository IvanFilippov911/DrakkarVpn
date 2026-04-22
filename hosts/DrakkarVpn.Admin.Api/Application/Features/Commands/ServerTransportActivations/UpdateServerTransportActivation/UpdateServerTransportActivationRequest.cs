using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;

public sealed record UpdateServerTransportActivationRequest(
    Guid ServerId,
    Guid ActivationId,
    string RealityPublicKey,
    int LocalPriority) : IServersCommand<Unit>;
