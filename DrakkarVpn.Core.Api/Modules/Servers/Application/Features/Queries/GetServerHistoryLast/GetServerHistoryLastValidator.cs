using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistoryLast;

public sealed class GetServerHistoryLastValidator : AbstractValidator<GetServerHistoryLastRequest>
{
    public GetServerHistoryLastValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Minutes).InclusiveBetween(1, 2880);
    }
}