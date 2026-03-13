using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerHistory;

public sealed class GetPeerHistoryRequestValidator : AbstractValidator<GetPeerHistoryRequest>
{
    public GetPeerHistoryRequestValidator()
    {
        RuleFor(x => x.PeerId).NotEmpty();

        When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
        {
            RuleFor(x => x.ToUtc!.Value)
                .GreaterThanOrEqualTo(x => x.FromUtc!.Value)
                .WithMessage("toUtc must be >= fromUtc");
        });
    }
}