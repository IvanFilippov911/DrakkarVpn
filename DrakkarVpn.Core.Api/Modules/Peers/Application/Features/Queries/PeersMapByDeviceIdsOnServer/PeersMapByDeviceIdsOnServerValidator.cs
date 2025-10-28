using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.PeersMapByDeviceIdsOnServer;

public sealed class PeersMapByDeviceIdsOnServerValidator
    : AbstractValidator<PeersMapByDeviceIdsOnServerQuery>
{
    public PeersMapByDeviceIdsOnServerValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.DeviceIds).NotNull();
        RuleFor(x => x.DeviceIds.Count).GreaterThan(0);
        RuleForEach(x => x.DeviceIds).NotEmpty();
    }
}