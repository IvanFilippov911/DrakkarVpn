using DrakkarVpn.Admin.Api.Application.Features.Commands.Servers.RegisterServer;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;


public sealed class RegisterServerHandler
    : IRequestHandler<RegisterServerRequest, Guid>
{
    private readonly IServerManagementService _serverManagementService;
    private readonly IServerTransportManagementService _transportManagementService;

    public RegisterServerHandler(
        IServerManagementService serverManagementService,
        IServerTransportManagementService transportManagementService)
    {
        _serverManagementService = serverManagementService;
        _transportManagementService = transportManagementService;
    }

    public async Task<Guid> Handle(RegisterServerRequest c, CancellationToken ct)
    {
        var serverId = await _serverManagementService.RegisterAsync(
            c.Name,
            c.Region,
            c.PublicHost,
            c.PublicPort,
            c.AgentBaseUrl,
            c.AgentTokenEncrypted,
            c.MaxPeers,
            ct);

        if (c.TransportProfiles is null || c.TransportProfiles.Count == 0)
            return serverId;

        var attachInput = new AttachServerTransportProfilesInput(
            ServerId: serverId,
            Profiles: c.TransportProfiles
                .Select(x => new AttachServerTransportProfileItemInput(
                    x.TransportProfileId,
                    x.RealityPublicKey,
                    x.LocalPriority))
                .ToList(),
            ActivateProfileId: c.ActivateProfileId);

        await _transportManagementService.AttachProfilesAsync(attachInput, ct);
        return serverId;
    }
}