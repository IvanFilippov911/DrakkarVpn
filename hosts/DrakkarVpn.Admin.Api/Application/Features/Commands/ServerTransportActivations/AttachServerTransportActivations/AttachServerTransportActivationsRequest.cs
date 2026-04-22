using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransportActivations;

public sealed record AttachServerTransportActivationsRequest(
    Guid ServerId,
    IReadOnlyList<AttachServerTransportActivationRequestItem> Profiles,
    Guid? ActivateProfileId) : IServersCommand<Unit>;

public sealed record AttachServerTransportActivationRequestItem(
    Guid TransportProfileId,
    string RealityPublicKey,
    int LocalPriority);
