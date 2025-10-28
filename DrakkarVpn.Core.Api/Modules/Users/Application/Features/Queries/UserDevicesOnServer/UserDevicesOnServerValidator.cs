using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.UserDevicesOnServer;

public sealed class GetUserDevicesOnServerValidator : AbstractValidator<UserDevicesOnServerQuery>
{
    public GetUserDevicesOnServerValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}