using DrakkarVpn.Agent.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Agent.Application.Feature.Commands.ApplyServerTransport;

public sealed record ApplyServerTransportCommand(ApplyServerTransportRequestDto Body)
    : IRequest<AgentTransportApplyResult>;
