using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerHistory;

public sealed class GetPeerHistoryValidator 
    : AbstractValidator<GetPeerHistoryRequest>
{
    public GetPeerHistoryValidator()
    {
        RuleFor(x => x.PeerId)
            .NotEmpty();
        
        When(x => x.FromUtc.HasValue && x.ToUtc.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.FromUtc!.Value < x.ToUtc!.Value)
                .WithMessage("FromUtc must be earlier than ToUtc.");

            RuleFor(x => x)
                .Must(x =>
                {
                    var diff = x.ToUtc!.Value - x.FromUtc!.Value;
                    return diff.TotalMinutes >= 10 && diff.TotalDays <= 7;
                })
                .WithMessage("Range between FromUtc and ToUtc must be between 10 minutes and 7 days.");
        });
    }
}