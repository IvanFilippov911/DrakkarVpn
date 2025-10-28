using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.UserDevicesOnServer;

public sealed record UserDevicesOnServerQuery(
    Guid ServerId,
    Guid UserId
) : IRequest<IReadOnlyList<DeviceListItemDto>>;