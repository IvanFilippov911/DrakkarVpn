using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;

public sealed record GetVpnConfigRequest(
    long TelegramId,
    string DeviceId
) : IRequest<GetVpnConfigResponse>;