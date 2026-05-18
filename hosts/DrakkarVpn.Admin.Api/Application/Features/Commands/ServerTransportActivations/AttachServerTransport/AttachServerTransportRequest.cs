using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransport;

public sealed record AttachServerTransportRequest(
    Guid ServerId,
    IReadOnlyList<AttachServerTransportRequestItem> Profiles,
    Guid? ActivateProfileId) : IServersCommand<Unit>;

public sealed record AttachServerTransportRequestItem(
    Guid TransportProfileId,
    string RealityPublicKey,
    int LocalPriority);
