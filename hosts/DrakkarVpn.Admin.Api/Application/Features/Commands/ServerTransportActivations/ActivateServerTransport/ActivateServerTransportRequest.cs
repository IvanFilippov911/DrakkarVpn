using DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;
using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;

public sealed record ActivateServerTransportRequest(
    Guid ServerId,
    Guid ActivationId) : IServersCommand<ActivateServerTransportResultDto>;
