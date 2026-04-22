using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed record ActivateServerTransportActivationRequest(
    Guid ServerId,
    Guid ActivationId) : IServersCommand<Unit>;
