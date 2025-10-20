using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistory;

public sealed class GetServerHistoryValidator : AbstractValidator<GetServerHistoryRequest>
{
    public GetServerHistoryValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();

        RuleFor(x => x).Custom((q, ctx) =>
        {
            var toUtc   = (q.ToUtc   ?? DateTime.UtcNow).ToUniversalTime();
            var fromUtc = (q.FromUtc ?? toUtc.AddHours(-24)).ToUniversalTime();

            if (toUtc < fromUtc)
                ctx.AddFailure("to", "`to` must be >= `from`.");
            if ((toUtc - fromUtc) > TimeSpan.FromHours(48))
                ctx.AddFailure("range", "Maximum range is 48 hours.");
        });
    }
}