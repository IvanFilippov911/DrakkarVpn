using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Errors;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportActivations;

internal static class ServerTransportActivationErrors
{
    public static DomainException ServerNotFound(Guid serverId)
        => new(
            DomainArea.Servers,
            ServerTransportActivationErrorCodes.ServerNotFound,
            $"Server '{serverId}' was not found.");

    public static DomainException ActivationNotFound(Guid serverId, Guid activationId)
        => new(
            DomainArea.Servers,
            ServerTransportActivationErrorCodes.ActivationNotFound,
            $"Activation '{activationId}' was not found on server '{serverId}'.");
}
