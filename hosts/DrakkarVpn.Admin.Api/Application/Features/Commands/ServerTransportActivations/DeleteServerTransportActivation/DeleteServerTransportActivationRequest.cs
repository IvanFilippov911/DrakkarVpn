using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;

public sealed record DeleteServerTransportActivationRequest(
    Guid ServerId,
    Guid ActivationId) : IServersCommand<Unit>;
