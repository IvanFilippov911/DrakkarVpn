using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RegisterUser;

public sealed class RegisterOrGetByTelegramHandler
    : IRequestHandler<RegisterRequest, RegisterUserResultDto>
{
    private readonly IUserRegistrationService _reg;

    public RegisterOrGetByTelegramHandler(IUserRegistrationService reg) => _reg = reg;

    public async Task<RegisterUserResultDto> Handle(RegisterRequest req, CancellationToken ct)
    {
        var tgId = req.regCommand.TelegramId;

        var dto = await _reg.RegisterOrGetAsync(
            telegramId: tgId,
            nowUtc: DateTime.UtcNow,
            ct: ct);

        return new RegisterUserResultDto(IsNewUser: dto.IsNew);
    }
}