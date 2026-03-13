using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Validation;

public sealed class TariffCreateDtoValidator : AbstractValidator<TariffCreateDto>
{
    public TariffCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Duration)
            .Must(d => d >= TimeSpan.FromDays(1))
            .WithMessage("Duration must be at least 1 day");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DefaultMaxDevices)
            .InclusiveBetween(1, 100);
    }
}