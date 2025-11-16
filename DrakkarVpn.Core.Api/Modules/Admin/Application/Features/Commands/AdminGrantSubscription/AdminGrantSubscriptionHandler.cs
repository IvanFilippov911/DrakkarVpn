using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.CreateSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.RenewSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeSubscriptionDevicesLimit;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.MarkUserInternal;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.AdminGrantSubscription;

public sealed class AdminGrantSubscriptionHandler 
    : IRequestHandler<AdminGrantSubscriptionCommand, SubscriptionDto>
{
    private readonly IMediator _mediator;
    private readonly IAppUserRepository _users;

    public AdminGrantSubscriptionHandler(IMediator mediator, IAppUserRepository users)
    {
        _mediator = mediator;
        _users    = users;
    }

    public async Task<SubscriptionDto> Handle(AdminGrantSubscriptionCommand cmd, CancellationToken ct)
    {
        if (cmd.MarkUserInternal)
        {
            var ok = await _mediator.Send(
                new MarkUserInternalCommand(
                    UserId:    cmd.UserId,
                    IsInternal: true),
                ct);

            if (!ok)
                throw new InvalidOperationException("User not found");
        }

        var active = await _mediator.Send(
            new GetActiveSubscriptionByUserRequest(cmd.UserId), ct);
        
        if (active is not null)
        {
            return await _mediator.Send(
                new RenewSubscriptionRequest(
                    SubscriptionId: active.Id,
                    TariffId:       cmd.TariffId,
                    DeviceCount:    cmd.DeviceCount 
                ),
                ct);
        }
        
        return await _mediator.Send(
            new CreateSubscriptionRequest(cmd.UserId, cmd.TariffId, cmd.DeviceCount),
            ct);
    }
}