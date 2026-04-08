using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RegisterUser;

public sealed class RegisterOrGetByTelegramHandler
    : IRequestHandler<RegisterRequest, RegisterUserResultDto>
{
    private readonly IUserRegistrationService _reg;
    private readonly IGrantTrialSubscriptionService _trial;

    public RegisterOrGetByTelegramHandler(
        IUserRegistrationService reg,
        IGrantTrialSubscriptionService trial)
    {
        _reg = reg;
        _trial = trial;
    }

    public async Task<RegisterUserResultDto> Handle(RegisterRequest req, CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;
        var tgId = req.regCommand.TelegramId;

        var dto = await _reg.RegisterOrGetAsync(
            telegramId: tgId,
            nowUtc: nowUtc,
            ct: ct);

        if (dto.IsNew)
            await _trial.TryGrantForNewUserAsync(dto.UserId, nowUtc, ct);

        return new RegisterUserResultDto(IsNewUser: dto.IsNew);
    }
}