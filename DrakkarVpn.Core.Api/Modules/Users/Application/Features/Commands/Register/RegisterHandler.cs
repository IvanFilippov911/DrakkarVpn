using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using MediatR;

public sealed class RegisterOrGetByTelegramHandler 
    : IRequestHandler<RegisterRequest, RegisterUserResponse>
{
    private readonly IAppUserRepository _repo;

    public RegisterOrGetByTelegramHandler(IAppUserRepository repo) => _repo = repo;

    public async Task<RegisterUserResponse> Handle(RegisterRequest req, CancellationToken ct)
    {
        var tgId = req.regCommand.TelegramId;

        var existing = await _repo.GetByTelegramIdAsync(tgId, ct);
        if (existing is not null)
            return new RegisterUserResponse(false);

        var created = AppUser.CreateNew(tgId, DateTime.UtcNow);
        await _repo.AddAsync(created, ct);

        return new RegisterUserResponse(true);
    }
}