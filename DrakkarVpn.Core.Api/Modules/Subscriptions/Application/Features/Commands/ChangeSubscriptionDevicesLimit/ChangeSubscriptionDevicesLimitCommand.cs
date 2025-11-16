
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeSubscriptionDevicesLimit;

public sealed record ChangeSubscriptionDevicesLimitCommand(
    Guid SubscriptionId,
    int NewMaxDevices
) : IRequest;